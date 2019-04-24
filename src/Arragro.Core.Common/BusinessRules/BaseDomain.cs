using Arragro.Core.Common.Enums;
using Arragro.Core.Common.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.Core.Common.BusinessRules
{
    internal abstract class BaseDomain<TModel> : BaseDomainServiceless<TModel> where TModel : class, IDictionaryKey, new()
    {
        protected readonly IBaseService<TModel> _service;

        protected override void LoadFromSql(Stopwatch timer)
        {
            DictionaryData = new ConcurrentDictionary<string, TModel>(_service.All().ToDictionary(x => x.DictionaryKey.ToString(), x => x));
            _log.Info(string.Format("All SQL Data Loaded in {0}ms", timer.ElapsedMilliseconds));
            timer.Restart();
        }

        protected BaseDomain(
            ApplicationProperties applicationProperties, IFileCacheHelper fileCacheHelper,
            IBaseService<TModel> service, WarmupTypes warmupType = WarmupTypes.Sql) :
                base(applicationProperties, fileCacheHelper, warmupType, false)
        {
            _service = service;
            ConstructData();
        }

        public override async Task ReloadData()
        {
            var data = await Task.Run(() => _service.All().ToDictionary(x => x.DictionaryKey.ToString(), x => x)).ConfigureAwait(continueOnCapturedContext: false);
            lock (_locker)
            {
                DictionaryData = new ConcurrentDictionary<string, TModel>(data);
            }
        }

        protected TModel Insert(TModel model)
        {
            var result = _service.ValidateAndInsert(model);
            UpdateCachedData(model);
            return result;
        }

        protected bool Update(TModel model)
        {
            var result = _service.ValidateAndUpdate(model);
            UpdateCachedData(model);
            return result;
        }

        protected new bool Delete(TModel model)
        {
            var output = _service.Delete(model);
            base.Delete(model);
            return output;
        }

        public override async Task<IEnumerable<TModel>> UpdateFromModification()
        {
            var timer = new Stopwatch();
            timer.Start();

            var changed = await Task.Run(() =>
            {
                try
                {
                    if (Data.Any())
                        return _service.AllChanged(Data.Max(x => x.LastWriteDate));
                    return _service.All();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Error updating {0} dataset", typeof(TModel)), ex);
                    throw;
                }
            }).ConfigureAwait(continueOnCapturedContext: false);

            UpdateFromModification(changed);

            if (changed.Any())
            {
                timer.Stop();
                _log.Info(string.Format("Completed the updating of modified data in {0}ms with {1} records updated", timer.ElapsedMilliseconds, changed.Count()));
            }

            return changed;
        }
    }
}
