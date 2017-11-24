using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Arragro.Core.Common.Helpers
{
    public static class ObjectHelpers
    {
        private static readonly IDictionary<Type, PropertyInfo[]> KeyProperties = new Dictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] GetKeyProperties<TModel>()
        {
            var type = typeof(TModel);
            if (!KeyProperties.ContainsKey(type))
            {
                var name = type.Name;
                var properties = type.GetTypeInfo().GetProperties();
                var key = properties.Where(x => x.IsDefined(typeof(KeyAttribute), true));

                if (!key.Any())
                    key = properties.Where(x => x.Name == name + "Id");

                if (!key.Any())
                    key = properties.Where(x => x.Name == "Id");

                if (key == null)
                    throw new Exception("Cannot find Key, use Id, {Type}Id, or Key attribute");

                KeyProperties.Add(type, key.ToArray());
            }

            return KeyProperties[type];
        }


        public static object[] GetKeyPropertyValues<TModel>(TModel model)
        {
            return GetKeyProperties<TModel>().Select(x => x.GetValue(model)).ToArray();
        }


        public static MethodCallExpression GetFindWhereClause<TModel>(IQueryable<TModel> query, params object[] ids)
        {
            var keyProperties = GetKeyProperties<TModel>();

            // See: http://msdn.microsoft.com/en-us/library/bb882637.aspx
            // Get the Key Property
            var pe = Expression.Parameter(typeof(TModel));
            var equals = new List<Expression>();
            equals.Add(query.Expression);

            for (int i = 0; i < keyProperties.Length; i++)
            {
                var keyProperty = keyProperties[i];
                // Set the left side of the where clause (the property)
                var left = Expression.Property(pe, keyProperty.Name);
                // Set the right side of the where clause (the value)
                var right = Expression.Constant(ids[i], keyProperty.PropertyType);
                // Specify the type of where clause
                equals.Add(Expression.Lambda<Func<TModel, bool>>(Expression.Equal(left, right), new ParameterExpression[] { pe }));
            }

            // Create the where clause as an expression tree
            var whereClause =
                Expression.Call(
                            typeof(Queryable),
                            "Where",
                            new Type[] { query.ElementType },
                            equals.ToArray());
            return whereClause;
        }

        public static IList<ValidationResult> ValidateModelProperties<TModel>(this TModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
