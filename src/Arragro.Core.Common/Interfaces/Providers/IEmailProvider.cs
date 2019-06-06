using Arragro.Core.Common.Models;
using System;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IEmailProvider
    {
        Task<Guid> SendEmailAsync(EmailMessage emailMessage);
    }
}
