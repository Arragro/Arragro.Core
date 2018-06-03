using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionListContainer
    {
        public string Key { get; set; }
        public string PropertyName { get; set; }
        public int? Index { get; set; }
        public List<RulesExceptionListContainer> RulesExceptionListContainers { get; set; }

        public string KeyIndex
        {
            get
            {
                return Char.ToLowerInvariant(PropertyName[0]) + PropertyName.Substring(1) + (Index.HasValue ? $"[{Index}]" : "");
            }
        }

        public RulesExceptionListContainer(string key, RulesException rulesException)
        {
            Key = key;
            PropertyName = rulesException.PropertyName;
            Index = rulesException.Index;
            RulesExceptionListContainers = new List<RulesExceptionListContainer>();
        }
    }

    public class RulesExceptionDto
    {
        public IDictionary<string, List<object>> Errors { get; protected set; }
        public List<string> ErrorMessages { get; protected set; }
        public List<RulesExceptionListContainer> RulesExceptionListContainers { get; private set; }

        public RulesExceptionDto()
        {
            Errors = new Dictionary<string, List<object>>();
            ErrorMessages = new List<string>();
            RulesExceptionListContainers = new List<RulesExceptionListContainer>();
        }

        private string CamelCase(string value)
        {
            var split = value.Split('.');
            for (var i = 0; i < split.Length; i++)
            {
                split[i] = Char.ToLowerInvariant(split[i][0]) + split[i].Substring(1);
            }
            return String.Join(".", split);
        }

        private RulesExceptionListContainer FindRulesExceptionListContainer(string[] keySplit, int index, RulesException rulesException, List<RulesExceptionListContainer> rulesExceptionListContainers)
        {
            var rulesExceptionListContainer = rulesExceptionListContainers.SingleOrDefault(x => x.KeyIndex == keySplit[index] || (x.Key == "" && x.KeyIndex == keySplit[index]));
            if (rulesExceptionListContainer != null && rulesExceptionListContainer.Index.HasValue)
            {
                var nextIndex = index + 1;
                var findRulesExceptionListContainer = FindRulesExceptionListContainer(keySplit, nextIndex, rulesException, rulesExceptionListContainer.RulesExceptionListContainers);
                if (findRulesExceptionListContainer != null)
                    return findRulesExceptionListContainer;
                else if (keySplit[nextIndex].EndsWith("]"))
                {
                    var newRulesExceptionListContainers = new RulesExceptionListContainer("", rulesException);
                    rulesExceptionListContainer.RulesExceptionListContainers.Add(newRulesExceptionListContainers);
                    findRulesExceptionListContainer = FindRulesExceptionListContainer(keySplit, nextIndex, rulesException, rulesExceptionListContainer.RulesExceptionListContainers);
                    if (findRulesExceptionListContainer != null)
                        return findRulesExceptionListContainer;
                }
            }
            return rulesExceptionListContainer;
        }

        private void FindAndAddRulesExceptionListContainer(string key, string[] keySplit, RulesException rulesException)
        {
            var rulesExceptionListContainer = FindRulesExceptionListContainer(keySplit, 0, rulesException, RulesExceptionListContainers);
            if (rulesExceptionListContainer == null)
            {
                var newRulesExceptionListContainers = new RulesExceptionListContainer("", rulesException);
                newRulesExceptionListContainers.RulesExceptionListContainers.Add(new RulesExceptionListContainer(key, rulesException));
                RulesExceptionListContainers.Add(newRulesExceptionListContainers);
            }
            else
            {
                rulesExceptionListContainer.RulesExceptionListContainers.Add(new RulesExceptionListContainer(key, rulesException));
            }
        }

        private void ProcessEnumerableRulesException(string key, KeyValuePair<string, List<object>> error, RulesException rulesException)
        {
            var keySplit = key.Split('.');
            if (!rulesException.IsEnumerable && !keySplit.Any(x => x.EndsWith("]")))
            {
                Errors.Add(key, error.Value);
            }
            else if (rulesException.IsEnumerable && keySplit.Count(x => x.EndsWith("]")) == 1)
            {
                FindAndAddRulesExceptionListContainer(key, keySplit, rulesException);
            }
            else if ((!rulesException.IsEnumerable && keySplit.Any(x => x.EndsWith("]"))) ||
                     keySplit.Count(x => x.EndsWith("]")) > 1)
            {
                FindAndAddRulesExceptionListContainer(key, keySplit, rulesException);
            }
            else
                RulesExceptionListContainers.Add(new RulesExceptionListContainer(key, rulesException));
        }

        protected void ProcessDictionaries(IEnumerable<RulesException> rulesExceptions)
        {
            foreach (var rulesException in rulesExceptions)
            {
                var errors = rulesException.GetErrorDictionary().ToList();
                foreach (var error in errors)
                {
                    string key = string.IsNullOrEmpty(rulesException.Prefix) ? CamelCase(error.Key) : $"{CamelCase(rulesException.Prefix)}.{CamelCase(error.Key)}";

                    ProcessEnumerableRulesException(key, error, rulesException);
                }
            }
        }

        public RulesExceptionDto(RulesException rulesException) : this()
        {
            Errors = rulesException.GetErrorDictionary();
            ErrorMessages.AddRange(rulesException.ErrorMessages);
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            output.AppendLine("The following error is against this object:\n");
            foreach (var error in ErrorMessages)
                output.AppendLine($"\t{error}");

            return output.ToString();
        }

        public RulesExceptionDto(IEnumerable<RulesException> rulesExceptions) : this()
        {
            ProcessDictionaries(rulesExceptions);
            rulesExceptions.SelectMany(x => x.ErrorMessages).ToList().ForEach(x => ErrorMessages.Add(x));
        }
    }
}
