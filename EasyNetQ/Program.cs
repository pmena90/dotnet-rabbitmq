// See https://aka.ms/new-console-template for more information
using Domain.Entities;
using Infrastructure.Common;

var _messageService = new EasyQueueService();

// SendReceive simple pattern
for (int i = 0; i < 10; i++)
{
    var m = new TextMessage
    {
        Id = i,
        Text = i + "Hello from EasyNetq",
    };

    await _messageService.PublishAsync(m);
}

var m2 = new CardPaymentRequestMessage
{
    Id = 1,
    Amount = 1,
    CardHolderName = "Pavel",
    CardNumber = "4242 4242 4242 4242",
    ExpiryDate = "11/24"
};

await _messageService.PublishAsync(m2);

// RPC pattern
var message = new RpcRequestMessage
{
    Id = 111,
    Text = "Hello waitting response",
};

await _messageService.RpcRequestAsync(message);

Console.WriteLine("Press a key for exit");
Console.Read();