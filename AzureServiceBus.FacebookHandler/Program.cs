using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureServiceBus.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureServiceBus.FacebookHandler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("FACEBOOK HANDLER");
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();

                    var secretClient = new SecretClient(
                        new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/"), 
                        new EnvironmentCredential());
                    
                    config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<MessageHandler>();

                    services.AddSingleton<IServiceBusSubscriptionService, ServiceBusSubscriptionService>();

                });
    }
}
