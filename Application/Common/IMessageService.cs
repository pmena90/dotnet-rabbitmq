using Domain.Common;
using Domain.Entities;

namespace Application.Common
{
    public interface IMessageService
    {
        Task SendReceiveSendAsync(ICustomMessage message);

        Task SendReceiveReceiveAsync();

        Task RpcRequestAsync(RpcRequestMessage message);

        Task RpcRespondAsync();

        Task PubSubPublishAsync(ICustomMessage m, string topic = "");

        Task PubSubSubscribeAsync(string receiverId, string topic = "");
    }
}