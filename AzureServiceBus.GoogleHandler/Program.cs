using System;
using System.Threading.Tasks;
using AzureServiceBus.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureServiceBus.GoogleHandler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("GOOGLE HANDLER");
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<MessageHandler>();

                    services.AddSingleton<IServiceBusSubscriptionService, ServiceBusSubscriptionService>();
                });
    }
}
