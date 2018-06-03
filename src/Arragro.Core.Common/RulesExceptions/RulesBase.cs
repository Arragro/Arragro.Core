using Arragro.Core.Common.Helpers;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Arragro.Core.Common.RulesExceptions
{
    public interface IRulesBase<TParamerterType> where TParamerterType : class
    {
        void Validate(TParamerterType parameters);
    }

    public class RulesBase<TModel>
        where TModel : class
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

        protected RulesExceptionCollection ValidateModelPropertiesAndBuildRulesExceptionCollection<TParameterType>(object model, TParameterType parameterType, string prefix = "", RulesExceptionCollection rulesExceptionCollection = null) where TParameterType : class
        {
            var type = model.GetType();
            var ruleBaseTypes = type.GetProperties().Where(x => typeof(IRulesBase<TParameterType>).IsAssignableFrom(x.PropertyType));
            var ruleBasesTypes = type.GetProperties().Where(x => typeof(IEnumerable<IRulesBase<TParameterType>>).IsAssignableFrom(x.PropertyType));

            if (rulesExceptionCollection == null)
                rulesExceptionCollection = new RulesExceptionCollection(new[] { RulesException });

            foreach (var rb in ruleBaseTypes)
            {
                var value = rb.GetValue(model);
                var rbPrefix = string.IsNullOrEmpty(prefix) ? rb.Name : $"{prefix}.{rb.Name}";
                if (value != null)
                {
                    rb.PropertyType.GetInterface(typeof(IRulesBase<TParameterType>).Name).GetMethod("Validate").Invoke(value, new object[] { parameterType });
                    var rulesException = rb.PropertyType.BaseType.GetRuntimeField("RulesException").GetValue(value) as RulesException;
                    rulesException.Prefix = rbPrefix;
                    rulesException.PropertyName = rb.Name;
                    rulesExceptionCollection.RulesExceptions.Add(rulesException);

                    ValidateModelPropertiesAndBuildRulesExceptionCollection<TParameterType>(value, parameterType, rbPrefix, rulesExceptionCollection);
                }
            }

            foreach (var rbs in ruleBasesTypes)
            {
                var enumerable = rbs.GetValue(model) as IEnumerable<IRulesBase<TParameterType>>;
                var rbsPrefix = string.IsNullOrEmpty(prefix) ? rbs.Name : $"{prefix}.{rbs.Name}";
                if (enumerable != null)
                {
                    for (var i = 0; i < enumerable.Count(); i++)
                    {
                        var e = enumerable.ElementAt(i);
                        var iRbsPrefix = $"{rbsPrefix}[{i}]";
                        e.Validate(parameterType);
                        var enumerableType = e.GetType();
                        var rulesException = enumerableType.BaseType.GetRuntimeField("RulesException").GetValue(e) as RulesException;
                        rulesException.Prefix = iRbsPrefix;
                        rulesException.IsEnumerable = true;
                        rulesException.PropertyName = rbs.Name;
                        rulesException.Index = i;

                        rulesExceptionCollection.RulesExceptions.Add(rulesException);
                        ValidateModelPropertiesAndBuildRulesExceptionCollection<TParameterType>(e, parameterType, iRbsPrefix, rulesExceptionCollection);
                    }
                }
            }

            return rulesExceptionCollection;
        }

        protected RulesExceptionCollection ValidateModelPropertiesAndBuildRulesExceptionCollection<TParameterType>(TModel model, TParameterType parameterType) where TParameterType : class
        {
            return ValidateModelPropertiesAndBuildRulesExceptionCollection<TParameterType>(model, parameterType, "", null);
        }

        protected IList<ValidationResult> ValidateModelProperties(TModel model)
        {
            return model.ValidateModelProperties();
        }
    }
}
