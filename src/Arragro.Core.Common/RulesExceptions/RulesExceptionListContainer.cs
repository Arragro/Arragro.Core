using Arragro.Core.Common.Helpers;
using System;
using System.Collections.Generic;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionListContainer
    {
        public bool IsRoot { get; set; } = false;
        public string Key { get; set; }
        public string PropertyName { get; set; }
        public string PropertyNameCamelCase { get; set; }
        public int? Index { get; set; }
        public IDictionary<string, List<object>> Errors { get; set; } = new Dictionary<string, List<object>>();
        public List<RulesExceptionListContainer> RulesExceptionListContainers { get; set; } = new List<RulesExceptionListContainer>();

        public string KeyIndex
        {
            get
            {
                return PropertyNameCamelCase + (Index.HasValue ? $"[{Index}]" : "");
            }
        }

        public RulesExceptionListContainer(RulesException rulesException)
        {
            IsRoot = true;
            PropertyName = rulesException.PropertyName;
            if (!string.IsNullOrEmpty(PropertyName))
                PropertyNameCamelCase = PropertyName.ToCamelCase();
            Index = rulesException.Index;
        }

        public RulesExceptionListContainer(string key, KeyValuePair<string, List<object>> error, RulesException rulesException)
        {
            IsRoot = false;
            Key = key;
            PropertyName = rulesException.PropertyName;
            if (!string.IsNullOrEmpty(PropertyName))
                PropertyNameCamelCase = PropertyName.ToCamelCase();
            Index = rulesException.Index;

            Errors.Add(error.Key.ToCamelCase(), error.Value);
        }

        public void AddError(KeyValuePair<string, List<object>> error)
        {
            Errors.Add(error.Key.ToCamelCase(), error.Value);
        }
    }
}
