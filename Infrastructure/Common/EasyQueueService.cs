using Application.Common;
using Domain.Common;
using Domain.Entities;
using EasyNetQ;

namespace Infrastructure.Common
{
    public class EasyQueueService : IMessageService
    {
        public async Task SendReceiveSendAsync(ICustomMessage message)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                await bus.SendReceive.SendAsync("my.paymentsqueue", message);

                Console.WriteLine("***SendReceive*** sent");
            }
        }

        public async Task SendReceiveReceiveAsync()
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                await bus.SendReceive.ReceiveAsync("my.paymentsqueue", x => x
                    .Add<TextMessage>(HandleTextMsg)
                    .Add<CardPaymentRequestMessage>(HandleCardMsg)
                );

                Console.WriteLine("***SendReceive*** Listening for messages. Hit <return> to quit");
                Console.ReadLine();
            }
        }

        public async Task RpcRequestAsync(RpcRequestMessage message)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                var task = bus.Rpc.RequestAsync<RpcRequestMessage, ResponseMessage>(message);

                await task.ContinueWith(x =>
                {
                    Console.WriteLine("RPC Request Response:");
                    Console.WriteLine(x.Result.AuthCode);
                });
            }
        }

        public async Task RpcRespondAsync()
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                await bus.Rpc.RespondAsync<RpcRequestMessage, ResponseMessage>(Responder);

                Console.WriteLine("***RPC*** Listening for messages. Hit <return> to quit");
                Console.ReadLine();
            }
        }

        public async Task PubSubPublishAsync(ICustomMessage message, string topic = "")
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                if (!string.IsNullOrEmpty(topic))
                {
                    await bus.PubSub.PublishAsync(message, topic);
                    Console.WriteLine("***PubSub Topic*** published msg");

                }

                await bus.PubSub.PublishAsync(message).ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        Console.WriteLine("***PubSub*** published msg");
                    }
                    else
                    {
                        throw new EasyNetQException("Message processing exception");
                    }
                });
            }
        }

        public async Task PubSubSubscribeAsync(string receiverId, string topic = "")
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"))
            {
                if (!string.IsNullOrEmpty(topic))
                {
                    await bus.PubSub.SubscribeAsync<ICustomMessage>(receiverId, HandleAllMsg, x => x.WithTopic("topic"));
                }
                await bus.PubSub.SubscribeAsync<ICustomMessage>(receiverId, message => Task.Factory.StartNew(() =>
                {
                    HandleAllMsg(message);
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

                Console.WriteLine("***PubSub*** Listening for messages. Hit <return> to quit");
                Console.ReadLine();
            }
        }

        private static void HandleAllMsg(ICustomMessage message)
        {
            var textMessage = message as TextMessage;
            var cardMessage = message as CardPaymentRequestMessage;

            if (cardMessage != null)
            {
                HandleCardMsg(cardMessage);
            }
            else if (textMessage != null)
            {
                HandleTextMsg(textMessage);
            }
        }

        private static void HandleTextMsg(TextMessage message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got Message: {0} ", message.Text);
            Console.ResetColor();
        }

        private static void HandleCardMsg(CardPaymentRequestMessage message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got Message: CardHolderName {0}, CardNumber {1}, ExpiryDate {2}, Amount {3}",
                message.CardHolderName, message.CardNumber, message.ExpiryDate, message.Amount);
            Console.ResetColor();
        }

        private static ResponseMessage Responder(RpcRequestMessage message)
        {
            Console.WriteLine("***RPC*** Respond Activated");

            return new ResponseMessage { AuthCode = "1234" };
        }
    }
}