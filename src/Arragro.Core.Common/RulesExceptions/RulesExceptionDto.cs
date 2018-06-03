using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionListContainer
    {
        public string PropertyName { get; set; }
        public int Index { get; set; }
        public string Path { get; set; }
        public List<RulesExceptionListContainer> RulesExceptionListContainers { get; set; } = null;
    }

    public class RulesExceptionDto
    {
        public IDictionary<string, List<object>> Errors { get; protected set; }
        public List<string> ErrorMessages { get; protected set; }

        public RulesExceptionDto()
        {
            Errors = new Dictionary<string, List<object>>();
            ErrorMessages = new List<string>();
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

        protected void ProcessDictionaries(IEnumerable<RulesException> rulesExceptions)
        {
            foreach (var rulesException in rulesExceptions)
            {
                var errors = rulesException.GetErrorDictionary().ToList();
                foreach (var error in errors)
                {
                    string key = string.IsNullOrEmpty(rulesException.Prefix) ? CamelCase(error.Key) : $"{CamelCase(rulesException.Prefix)}.{CamelCase(error.Key)}";
                    Errors.Add(key, error.Value);
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
