using System;
using System.Threading.Tasks;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureServiceBus.Infrastructure;
using AzureServiceBusPoc.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureServiceBusPoc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("PUBLISHER");
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
                        new EnvironmentCredential()); //Final implementation -> use new DefaultAzureCredential() because this one will go through all existing credential types

                    config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ProgramRunner>();

                    services.AddSingleton<IServiceBusSubscriptionService, ServiceBusSubscriptionService>();
                    services.AddScoped<IServiceBusPublisher, ServiceBusPublisher>();
                });

    }
}

