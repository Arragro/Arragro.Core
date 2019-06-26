using Arragro.Core.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionDto
    {
        public IDictionary<string, List<object>> Errors { get; protected set; }
        public List<string> ErrorMessages { get; protected set; }
        public List<RulesExceptionListContainer> RulesExceptionListContainers { get; protected set; }

        public RulesExceptionDto()
        {
            Errors = new Dictionary<string, List<object>>();
            ErrorMessages = new List<string>();
            RulesExceptionListContainers = new List<RulesExceptionListContainer>();
        }

        private RulesExceptionListContainer FindRulesExceptionListContainer(string[] keySplit, int index, KeyValuePair<string, List<object>> error, RulesException rulesException, List<RulesExceptionListContainer> rulesExceptionListContainers)
        {
            var rulesExceptionListContainer = rulesExceptionListContainers.SingleOrDefault(x => x.KeyIndex == keySplit[index] || (x.Key == "" && x.KeyIndex == keySplit[index]));
            if (rulesExceptionListContainer != null && rulesExceptionListContainer.Index.HasValue)
            {
                var nextIndex = index + 1;
                var findRulesExceptionListContainer = FindRulesExceptionListContainer(keySplit, nextIndex, error, rulesException, rulesExceptionListContainer.RulesExceptionListContainers);
                if (findRulesExceptionListContainer != null)
                    return findRulesExceptionListContainer;
                else if (keySplit[nextIndex].EndsWith("]"))
                {
                    var newRulesExceptionListContainers = new RulesExceptionListContainer(rulesException);
                    rulesExceptionListContainer.RulesExceptionListContainers.Add(newRulesExceptionListContainers);
                    findRulesExceptionListContainer = FindRulesExceptionListContainer(keySplit, nextIndex, error, rulesException, rulesExceptionListContainer.RulesExceptionListContainers);
                    if (findRulesExceptionListContainer != null)
                        return findRulesExceptionListContainer;
                }
            }
            return rulesExceptionListContainer;
        }

        private void FindAndAddRulesExceptionListContainer(string key, string[] keySplit, KeyValuePair<string, List<object>> error, RulesException rulesException)
        {
            var rulesExceptionListContainer = FindRulesExceptionListContainer(keySplit, 0, error, rulesException, RulesExceptionListContainers);
            if (rulesExceptionListContainer == null)
            {
                var newRulesExceptionListContainers = new RulesExceptionListContainer(rulesException);
                newRulesExceptionListContainers.AddError(error);
                RulesExceptionListContainers.Add(newRulesExceptionListContainers);
            }
            else
            {
                rulesExceptionListContainer.AddError(new KeyValuePair<string, List<object>>(key.Replace(rulesExceptionListContainer.KeyIndex + ".", ""), error.Value));
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
                FindAndAddRulesExceptionListContainer(key, keySplit, error, rulesException);
            }
            else if ((!rulesException.IsEnumerable && keySplit.Any(x => x.EndsWith("]"))) ||
                     keySplit.Count(x => x.EndsWith("]")) > 1)
            {
                FindAndAddRulesExceptionListContainer(key, keySplit, error, rulesException);
            }
            else
                RulesExceptionListContainers.Add(new RulesExceptionListContainer(key, error, rulesException));
        }

        protected void ProcessDictionaries(IEnumerable<RulesException> rulesExceptions, bool camelCaseKey = true)
        {
            foreach (var rulesException in rulesExceptions)
            {
                var errors = rulesException.GetErrorDictionary().ToList();
                foreach (var error in errors)
                {
                    string key = camelCaseKey ? 
                        string.IsNullOrEmpty(rulesException.Prefix) ? error.Key.ToCamelCaseFromDotNotation() : $"{rulesException.Prefix.ToCamelCaseFromDotNotation()}.{error.Key.ToCamelCaseFromDotNotation()}" :
                        string.IsNullOrEmpty(rulesException.Prefix) ? error.Key : $"{rulesException.Prefix}.{error.Key}";

                    ProcessEnumerableRulesException(key, error, rulesException);
                }
            }
        }

        public RulesExceptionDto(RulesException rulesException) : this()
        {
            Errors = rulesException.GetErrorDictionary();
            if (!string.IsNullOrEmpty(rulesException.ErrorMessage))
                ErrorMessages.Add(rulesException.ErrorMessage);
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

        public RulesExceptionDto(IEnumerable<RulesException> rulesExceptions, bool camelCaseKey = true) : this()
        {
            ProcessDictionaries(rulesExceptions, camelCaseKey);
            rulesExceptions.SelectMany(x => x.ErrorMessages).ToList().ForEach(x => ErrorMessages.Add(x));
        }
    }
}
