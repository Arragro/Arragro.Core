using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Arragro.Core.Common.Helpers
{
    public static class ObjectToKeyValuePairListExtension
    {

        public static List<KeyValuePair<string, string>> ToKeyValuePairList(this object source)
        {
            return source.ToKeyValuePairList<object>();
        }

        public static List<KeyValuePair<string, string>> ToKeyValuePairList<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var keyValuePairs = new List<KeyValuePair<string, string>>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToKeyValuePair<T>(property, source, keyValuePairs);

            return keyValuePairs;
        }

        private static void AddPropertyToKeyValuePair<T>(PropertyDescriptor property, object source, List<KeyValuePair<string, string>> keyValuePairs)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value) && value != null)
            {
                if (value is string)
                {
                    keyValuePairs.Add(new KeyValuePair<string, string>(property.Name, ((T)value).ToString()));
                }
                else if (value is IEnumerable)
                {
                    IEnumerable enumerable = (value as IEnumerable);
                    int count = 0;
                    foreach (var item in enumerable)
                    {
                        keyValuePairs.Add(new KeyValuePair<string, string>(property.Name + "[" + count.ToString() + "]", ((T)item).ToString()));
                        count++;
                    }
                }
                else
                    keyValuePairs.Add(new KeyValuePair<string, string>(property.Name, ((T)value).ToString()));
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
