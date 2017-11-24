using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionDto
    {
        public IDictionary<string, object> Errors { get; protected set; }
        public List<string> ErrorMessages { get; protected set; }

        public RulesExceptionDto()
        {
            Errors = new Dictionary<string, object>();
            ErrorMessages = new List<string>();
        }

        protected void processDictionaries(IEnumerable<RulesException> rulesExceptions)
        {
            foreach (var rulesException in rulesExceptions)
            {
                var errors = rulesException.GetErrorDictionary().ToList();
                foreach (var error in errors)
                {
                    object value;
                    if (Errors.TryGetValue(error.Key, out value))
                    {
                        Errors.Add($"{rulesException.TypeName}.{error.Key}", error.Value);
                    }
                    else
                        Errors.Add(error.Key, error.Value);
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
            processDictionaries(rulesExceptions);
            rulesExceptions.SelectMany(x => x.ErrorMessages).ToList().ForEach(x => ErrorMessages.Add(x));
        }
    }
}
