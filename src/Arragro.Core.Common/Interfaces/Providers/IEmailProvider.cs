using Arragro.Core.Common.Models;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IEmailProvider
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
