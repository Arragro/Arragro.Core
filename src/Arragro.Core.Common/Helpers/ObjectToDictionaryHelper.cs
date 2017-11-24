using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Arragro.Core.Common.Helpers
{
    public static class ObjectToDictionaryExtension
    {

        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, object> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);

            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, IDictionary<string, object> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value) && value != null)
            {
                if (value is string)
                {
                    dictionary[property.Name] = (T)value;
                }
                else if (value is IEnumerable)
                {
                    IEnumerable enumerable = (value as IEnumerable);
                    int count = 0;
                    foreach (var item in enumerable)
                    {
                        dictionary[property.Name + "[" + count.ToString() + "]"] = (T)item;
                        count++;
                    }
                }
                else
                    dictionary[property.Name] = (T)value;
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }
}
