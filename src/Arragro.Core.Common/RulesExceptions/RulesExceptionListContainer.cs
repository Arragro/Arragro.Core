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
        public KeyValuePair<string, List<object>> Error { get; set; }
        public List<RulesExceptionListContainer> RulesExceptionListContainers { get; set; }

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
            PropertyNameCamelCase = Char.ToLowerInvariant(PropertyName[0]) + PropertyName.Substring(1);
            Index = rulesException.Index;

            Error = default(KeyValuePair<string, List<object>>);
            RulesExceptionListContainers = new List<RulesExceptionListContainer>();
        }

        public RulesExceptionListContainer(string key, KeyValuePair<string, List<object>> error, RulesException rulesException)
        {
            IsRoot = false;
            Key = key;
            PropertyName = rulesException.PropertyName;
            PropertyNameCamelCase = Char.ToLowerInvariant(PropertyName[0]) + PropertyName.Substring(1);
            Index = rulesException.Index;

            Error = error;
            RulesExceptionListContainers = new List<RulesExceptionListContainer>();
        }
    }
}
