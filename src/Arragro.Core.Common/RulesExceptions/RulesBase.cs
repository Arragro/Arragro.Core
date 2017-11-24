using Arragro.Core.Common.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesBase<TModel> where TModel : class
    {
        public readonly RulesException<TModel> RulesException;

        public RulesBase()
        {
            RulesException = new RulesException<TModel>();
        }

        protected void ValidateModelPropertiesAndBuildRulesException(TModel model)
        {
            RulesException.ErrorsForValidationResults(ValidateModelProperties(model));
        }

        protected IList<ValidationResult> ValidateModelProperties(TModel model)
        {
            return model.ValidateModelProperties();
        }
    }
}
