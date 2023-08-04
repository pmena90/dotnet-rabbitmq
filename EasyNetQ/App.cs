using Application.Common;
using Domain.Entities;

namespace EasyNetQ_Sender
{
    public class App
    {
        private readonly IMessageService _messageService;

        public App(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task RunAsync(string[] args)
        {
            // SendReceive simple pattern
            for (int i = 0; i < 10; i++)
            {
                var m = new TextMessage
                {
                    Id = i,
                    Text = i + "Hello from EasyNetq",
                };

                await _messageService.PubSubPublishAsync(m);
            }

            var m2 = new CardPaymentRequestMessage
            {
                Id = 1,
                Amount = 1,
                CardHolderName = "Pavel",
                CardNumber = "4242 4242 4242 4242",
                ExpiryDate = "11/24"
            };

            await _messageService.PubSubPublishAsync(m2);

            // RPC pattern
            var message = new RpcRequestMessage
            {
                Id = 111,
                Text = "Hello waitting response",
            };

            await _messageService.RpcRequestAsync(message);

            // Send Receive pattern
            var textMessagte = new TextMessage
            {
                Id = 1,
                Text = "Hello from EasyNetq",
            };

            await _messageService.SendReceiveSendAsync(m2);
            await _messageService.SendReceiveSendAsync(textMessagte);
            await _messageService.SendReceiveSendAsync(m2);

            // publish to a topic
            //await _messageService.PubSubPublishAsync(m2, "payment.cardpayment");

            Console.WriteLine("Press a key for exit");
            Console.Read();
        }
    }
}