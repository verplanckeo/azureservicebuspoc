using System;
using System.IO;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureServiceBus.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: FunctionsStartup(typeof(AzureServiceBus.Functions.Startup))]
namespace AzureServiceBus.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var hostingEnvironment = services
                .BuildServiceProvider()
                .GetService<IHostingEnvironment>();

            var currentDirectory = hostingEnvironment.ContentRootPath;
            var isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
            if (isLocal)
            {
                currentDirectory = Environment.CurrentDirectory;
            }

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            var builtConfig = configurationBuilder.Build();

            var secretClient = new SecretClient(
                new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/"),
                new DefaultAzureCredential());

            configurationBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());


            builtConfig = configurationBuilder.Build();

            var context = builder.GetContext();
            var asbConn = context.Configuration["ConnectionStrings:AzureServiceBus"];

            services.AddSingleton(builtConfig);
            services.AddSingleton<IServiceBusSubscriptionService, ServiceBusSubscriptionService>();
            
            /*
             *The name of an app setting that contains the Service Bus connection string to use for this binding. If the app setting name begins with "AzureWebJobs",
             * you can specify only the remainder of the name. For example, if you set connection to "MyServiceBus", the Functions runtime looks for an app setting
             * that is named "AzureWebJobsMyServiceBus".
             * If you leave connection empty, the Functions runtime uses the default Service Bus connection string in the app setting that is named "AzureWebJobsServiceBus".
             */
            //var asbConnectionString = builtConfig["ConnectionStrings:AzureServiceBus"];
            //Environment.SetEnvironmentVariable(Constants.ServiceBus.ServiceBusKey, asbConnectionString);

            // More dependencies ...
        }

            
    }
}