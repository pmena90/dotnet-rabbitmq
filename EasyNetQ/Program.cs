// See https://aka.ms/new-console-template for more information
using Application.Common;
using EasyNetQ_Sender;
using Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<App>().RunAsync(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<IMessageService, EasyQueueService>();
            services.AddSingleton<App>();
        })
        .ConfigureAppConfiguration(app =>
        {
            app.AddJsonFile("appsettings.json");
        });
}