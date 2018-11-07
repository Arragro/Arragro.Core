using Arragro.Core.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesException : Exception
    {
        public readonly IList<RuleViolation> Errors = new List<RuleViolation>();
        public readonly IList<string> ErrorMessages = new List<string>();
        protected readonly Expression<Func<object, object>> ThisObject = x => x;
        protected readonly Type Type;
        public string Prefix { get; set; } = "";
        public bool IsEnumerable { get; set; } = false;
        public string PropertyName { get; set; }
        public int? Index { get; set; } = null;
        
        public RulesException()
        {
        }

        protected RulesException(Type type)
        {
            Type = type;
        }
        
        public string TypeName { get { return Type.Name; } }
        public string ErrorMessage { get; set; }

        protected RulesException(string message, RulesException rulesException) : base(message)
        {
            Errors = rulesException.Errors;
        }

        public RulesException(SerializationInfo info, StreamingContext context)
        {
            if (info != null)
            {
                foreach (SerializationEntry entry in info)
                {
                    switch (entry.Name)
                    {
                        case "ErrorMessage":
                            ErrorMessage = info.GetString("ErrorMessage");
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
                info.AddValue("ErrorMessage", this.ErrorMessage);
        }

        public void ErrorForModel(string message)
        {
            Errors.Add(new RuleViolation(ThisObject, message));
            ErrorMessages.Add(message);
        }

        protected void Add(RulesException errors)
        {
            foreach (var error in errors.Errors)
            {
                Errors.Add(error);
                if (string.IsNullOrEmpty(error.Prefix))
                    ErrorMessages.Add($"{error.Key} - {error.Message}");
                else
                    ErrorMessages.Add($"{error.Prefix} - {error.Key} - {error.Message}");
            }
        }

        public override string Message
        {
            get
            {
                var thisErrors = Errors.Where(x => x.Key == ExpressionHelper.GetExpressionText(ThisObject));
                var output = new StringBuilder(base.Message);

                if (thisErrors.Any())
                {
                    output.AppendLine("\n\n====================================");
                    output.AppendLine(ToString());
                }
                return output.ToString();
            }
        }

        public void ThrowException()
        {
            var thisErrors = Errors.Where(x => x.Key == ExpressionHelper.GetExpressionText(ThisObject));
            var output = new StringBuilder(base.Message);

            if (thisErrors.Any())
            {
                output.AppendLine("\n\n====================================");
                output.AppendLine(ToString());

                throw new RulesException(output.ToString(), this);
            }
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            output.AppendLine("The following error is against this object:\n");
            foreach (var error in Errors)
                output.AppendLine($"\t{error.Message}");

            return output.ToString();
        }

        public IDictionary<string, List<object>> GetErrorDictionary()
        {
            var dict = new Dictionary<string, List<object>>();
            foreach (var error in Errors)
            {
                if (dict.ContainsKey(error.KeyValuePair.Key))
                    dict[error.KeyValuePair.Key].Add(error.KeyValuePair.Value);
                else
                    dict.Add(error.KeyValuePair.Key, new List<object> { error.KeyValuePair.Value });
            }
            return dict;
        }

        public void ErrorFor(
            string propertyName,
            string message, string prefix = "")
        {
            var propertyError = new RuleViolation(propertyName, message, prefix);
            Errors.Add(propertyError);
        }
    }

    public class RulesException<TModel> : RulesException
    {
        public RulesException() : base(typeof(TModel)) { }

        public RulesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private RulesException(string message, RulesException rulesException) : base(message, rulesException) { }

        public void ErrorFor<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            string message, string prefix = "")
        {
            var propertyError = new RuleViolation(property, message, prefix);
            Errors.Add(propertyError);
        }

        public void ErrorsForValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            var type = typeof(TModel);
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    Errors.Add(new RuleViolation(memberName, validationResult.ErrorMessage));
                }
            }
        }

        public new void ThrowException()
        {
            var output = new StringBuilder(base.Message);

            if (Errors.Any() || ErrorMessages.Any())
            {
                output.AppendLine("\n\n====================================");
                output.AppendLine(ToString());
                throw new RulesException<TModel>(output.ToString(), this);
            }
        }
    }

    public static class RulesExceptionExtensions
    {
        public static bool ContainsErrorForProperty(this RulesException ex, string propertyName)
        {
            return ex.Errors.Any(x => x.Key.Contains(propertyName));
        }
    }
}
