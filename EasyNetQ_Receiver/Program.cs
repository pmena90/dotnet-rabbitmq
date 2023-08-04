// See https://aka.ms/new-console-template for more information
using Infrastructure.Common;

var _messageService = new EasyQueueService();
await _messageService.ReceiveAsync("EasyNetQ_Receiver_Client");

Console.WriteLine("Hello, World!");