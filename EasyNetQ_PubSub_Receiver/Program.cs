using Infrastructure.Common;

var _messageService = new EasyQueueService();
await _messageService.PubSubSubscribeAsync("EasyNetQ_Receiver_Client");