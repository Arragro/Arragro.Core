using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Arragro.Core.Common.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T enumerationValue)
                where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetTypeInfo().GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Any())
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs.ElementAt(0)).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }
}
