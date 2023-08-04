using Application.Common;
using Domain.Common;
using Domain.Entities;
using EasyNetQ;

namespace Infrastructure.Common
{
    public class EasyQueueService : IMessageService
    {
        public async Task RpcRequestAsync(RpcRequestMessage message)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest;timeout=10"))
            {
                var response = await bus.Rpc.RequestAsync<RpcRequestMessage, ResponseMessage>(message);

                Console.WriteLine("RPC Request Response:");
                Console.WriteLine(response.AuthCode);
            }
        }

        public async Task RpcRespondAsync()
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest;"))
            {
                await bus.Rpc.RespondAsync<RpcRequestMessage, ResponseMessage>(Responder);

                Console.WriteLine("RPC Response Send:");
            }
        }

        public async Task PublishAsync(ICustomMessage message)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                await bus.PubSub.PublishAsync(message).ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        Console.WriteLine("published msg");
                    }
                    else
                    {
                        throw new EasyNetQException("Message processing exception");
                    }
                });
            }
        }

        public async Task ReceiveAsync(string receiverId)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                await bus.PubSub.SubscribeAsync<ICustomMessage>(receiverId, message => Task.Factory.StartNew(() =>
                {
                    HandleTextMsg(message);
                }).ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        //Console.WriteLine("Finished Processing all msgs");
                    }
                    else
                    {
                        throw new EasyNetQException("Message processing exception");
                    }
                }));

                Console.WriteLine("Listening for messages. Hit <return> to quit");
                Console.ReadLine();
            }
        }

        private static void HandleTextMsg(ICustomMessage message)
        {
            var textMessage = message as TextMessage;
            var cardMessage = message as CardPaymentRequestMessage;

            if (cardMessage != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Got Message: CardHolderName {0}, CardNumber {1}, ExpiryDate {2}, Amount {3}",
                    cardMessage.CardHolderName, cardMessage.CardNumber, cardMessage.ExpiryDate, cardMessage.Amount);
                Console.ResetColor();
            }
            else if (textMessage != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Got Message: {0} ", textMessage.Text);
                Console.ResetColor();
            }
        }

        private static ResponseMessage Responder(RpcRequestMessage message)
        {
            return new ResponseMessage { AuthCode = "1234" };
        }
    }
}