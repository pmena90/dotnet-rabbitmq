using Domain.Common;

namespace Application.Common
{
    public interface IMessageService
    {
        Task PublishAsync(ICustomMessage m);

        Task ReceiveAsync(string receiverId);
    }
}