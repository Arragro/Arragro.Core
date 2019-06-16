using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IEmailClient<TMessage> where TMessage : class
    {
        Task<bool> DeleteMessageAsync(string messageId);
        Task<TMessage> GetMessageAsync(string messageId);
        Task<IEnumerable<TMessage>> GetMessagesAsync();
    }
}
