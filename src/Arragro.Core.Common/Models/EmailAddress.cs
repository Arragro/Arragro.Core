namespace Arragro.Core.Common.Models
{
    public class EmailAddress
    {
        public EmailAddress(string email, string name = null)
        {
            Email = email;
            Name = name;
        }

        public EmailAddress(string email)
        {
            email.Trim();

            if (email.Contains("<") && email.Contains(">"))
            {
                var name = email.Substring(0, email.IndexOf('<') - 1);
                var emailAddress = email.Substring(email.IndexOf('<') + 1, email.IndexOf('>') - email.IndexOf('<') - 1);

                Name = name;
                Email = emailAddress;
            }
            else
            {

                Email = email;
            }
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}
