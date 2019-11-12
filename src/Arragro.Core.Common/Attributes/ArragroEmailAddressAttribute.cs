using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Arragro.Core.Common.Attributes
{
    public class ArragroEmailAddressAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var emailaddress = value.ToString();
            try
            {
                var m = new MailAddress(emailaddress);

                var regexp = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

                return Regex.IsMatch(emailaddress, regexp);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
