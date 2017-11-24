using Arragro.Core.Common.Enums;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.Core.Common.BusinessRules
{
    public abstract class BaseDomainServiceless<TModel> where TModel : class, IDictionaryKey, new()
    {
        protected readonly ApplicationProperties _applicationProperties;
        protected readonly WarmupTypes _warmupType;

        protected static object _locker = new object();
        protected static bool _hasBeenConstruted = false;

        protected readonly IFileCacheHelper _fileCacheHelper;
        protected readonly ILog _log = LogManager.GetLogger(typeof(TModel));

        public abstract string Key { get; set; }

        protected static ConcurrentDictionary<string, TModel> DictionaryData { get; set; }

        public TModel GetDictionaryData(string key)
        {
            return DictionaryData[key];
        }

        public IQueryable<TModel> Data
        {
            get
            {
                return DictionaryData.Select(x => x.Value).AsQueryable();
            }
        }

        protected abstract void LoadFromSql(Stopwatch timer);

        public abstract Task ReloadData();

        protected void LoadFromFile(Stopwatch timer)
        {
            DictionaryData = new ConcurrentDictionary<string, TModel>(_fileCacheHelper.LoadSavedData<TModel>(_applicationProperties.ApplicationPath).Result);
            _log.Info(string.Format("All File Data Loaded in {0}ms", timer.ElapsedMilliseconds));
            timer.Restart();
            UpdateFromModification().Wait();
        }

        public static void ResetData()
        {
            lock (_locker)
            {
                if (DictionaryData != null && DictionaryData.Any())
                    DictionaryData.Clear();
                _hasBeenConstruted = false;
            }
        }

        protected void UpdateCachedData(TModel newData)
        {
            if (DictionaryData.ContainsKey(newData.DictionaryKey))
                DictionaryData[newData.DictionaryKey] = newData;
            else
                DictionaryData.TryAdd(newData.DictionaryKey, newData);
        }

        protected bool Delete(TModel model)
        {
            TModel removeModel;
            return DictionaryData.TryRemove(model.DictionaryKey, out removeModel);
        }

        public void RemoveDeleted(IEnumerable<string> changed)
        {
            foreach (var change in changed)
            {
                if (DictionaryData.ContainsKey(change.ToString()))
                {
                    TModel model;
                    DictionaryData.TryRemove(change.ToString(), out model);
                }
            }
        }

        public void UpdateFromModification(IEnumerable<TModel> changed)
        {
            var tempDict = new Dictionary<string, TModel>(DictionaryData);

            foreach (var change in changed)
            {
                if (tempDict.ContainsKey(change.DictionaryKey))
                    tempDict[change.DictionaryKey] = change;
                else
                    tempDict.Add(change.DictionaryKey, change);
            }

            DictionaryData = new ConcurrentDictionary<string, TModel>(tempDict);
        }

        public abstract Task<IEnumerable<TModel>> UpdateFromModification();

        public async Task SaveToFile()
        {
            var type = typeof(TModel);
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                _log.Info(string.Format("\tSaving {0} to disk", type.Name));
                List<TModel> data;
                lock (_locker)
                {
                    data = Data.ToList();
                }
                await _fileCacheHelper.SaveData(data, _applicationProperties.ApplicationPath).ConfigureAwait(continueOnCapturedContext: false);
                _log.Info(string.Format("File Data {0} Saved in {1}ms", typeof(TModel).Name, timer.ElapsedMilliseconds));
                timer.Restart();
                data.Clear();
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Something has gone wrong when saving a file for type {0}", type.Name), ex);
                throw;
            }
        }

        protected void ConstructData()
        {
            if (!_hasBeenConstruted)
            {
                lock (_locker)
                {
                    if (!_hasBeenConstruted)
                    {
                        var timer = new Stopwatch();
                        timer.Start();
                        
                        try
                        {
                            switch (_warmupType)
                            {
                                case WarmupTypes.Sql:
                                    LoadFromSql(timer);
                                    break;
                                case WarmupTypes.File:
                                    LoadFromFile(timer);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            LoadFromFile(timer);
                            _log.Error("Something went wrong retrieving data", ex);
                        }

                        _hasBeenConstruted = true;
                        timer.Restart();
                    }
                }
            }
        }

        public BaseDomainServiceless(ApplicationProperties applicationProperties, IFileCacheHelper fileCacheHelper, WarmupTypes warmupType = WarmupTypes.Sql)
        {
            _warmupType = warmupType;
            _fileCacheHelper = fileCacheHelper;
            _applicationProperties = applicationProperties;
        }

        protected BaseDomainServiceless(ApplicationProperties applicationProperties, IFileCacheHelper fileCacheHelper, WarmupTypes warmupType = WarmupTypes.Sql, bool constructData = false)
        {
            _warmupType = warmupType;
            _fileCacheHelper = fileCacheHelper;
            _applicationProperties = applicationProperties;

            if (constructData)
                ConstructData();
        }
    }
}
