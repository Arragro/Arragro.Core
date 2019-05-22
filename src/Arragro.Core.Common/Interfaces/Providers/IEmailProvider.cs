using Arragro.Core.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IEmailProvider
    {
        Task SendEmailAsync(string subject, string text, string html, EmailAddress from, List<EmailAddress> tos, List<EmailAddress> ccs = null, List<EmailAddress> bccs = null);
        Task SendEmailAsync(string subject, string text, string html, List<EmailAddress> tos, List<EmailAddress> ccs = null, List<EmailAddress> bccs = null);
    }
}
