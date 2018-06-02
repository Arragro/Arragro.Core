using Arragro.Core.Common.Helpers;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Arragro.Core.Common.RulesExceptions
{
    public interface IRulesBase
    {
        void Validate(bool throwException = true);
    }

    public class RulesBase<TModel> where TModel : class
    {
        public readonly RulesException<TModel> RulesException;

        public RulesBase()
        {
            RulesException = new RulesException<TModel>();
        }

        protected void ValidateModelPropertiesAndBuildRulesException(TModel model)
        {
            var type = model.GetType();
            var ruleBase = type.GetProperties().Where(x => typeof(IRulesBase).IsAssignableFrom(x.PropertyType));// type.GetProperties().Where(x => x.GetType().IsSubclassOf(typeof(RulesBase<>)));
            RulesException.ErrorsForValidationResults(ValidateModelProperties(model));
        }

        protected IList<ValidationResult> ValidateModelProperties(TModel model)
        {
            return model.ValidateModelProperties();
        }
    }
}
