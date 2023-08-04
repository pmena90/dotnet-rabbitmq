// See https://aka.ms/new-console-template for more information
using Infrastructure.Common;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

var config = configuration.Build();

var _messageService = new EasyQueueService(config);
await _messageService.RpcRespondAsync();

Console.WriteLine("Hello, World!");