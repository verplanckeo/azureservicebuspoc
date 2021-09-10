# ServiceBusPoc
This is a proof of concept to demonstrate how we'll send commands and publish events.
It uses an Azure Service Bus I configured on my personal Azure account. If you wish to use your own Service Bus, you'll need the following:
	+ A topic with the name "topicname"
	+ 2 queues: "facebook" and "google"
	+ 2 subscriptions which will redirect messages sent to the topic to the given facebook & google queue

	
The project consists of 3 executables:

## 1. AzureServiceBusPoc
This is the console application which sends commands and publishes events.

Sending a command sends it to the Facebook queue.
Publishing an event sends it to both the Facebook & Google queue.

## 2. FacebookHandler
This is the console application which listens and processes messages of the Facebook queue.


## 3. GoogleHandler
This is the console application which listens and processes messages of the Google queue.

# ServiceBusPoc - Azure keyvault
A second utility that was added is to demonstrate the use of Azure Keyvault. We're fetching the Azure Servicebus connectionstring from  Keyvault or from the local appsettings.json file.
The Azure Keyvault functionality has been added in a very transparent way. This is demonstrated using both the FacebookHandler & GoogleHandler.

## What do I mean with "transparent"?
With this I mean that the code within the services is identical no matter where the connectionstring is stored.

FacebookHandler & Publisher are using the Azure Keyvault to fetch the connectionstring for the AzureServiceBus service.
GoogleHandler is using its appsettings.json configuration file to fetch the connectionstring.

You'll notice that the code within both services to fetch the connectionstring is identical:
var connectionString = configuration["ConnectionStrings:AzureServiceBus"];

## How does the service know where it needs to fetch the connectionstring?
We bootstrap the application in program.cs by calling the 'addAzureKeyVault' extension method foreseen by Microsoft.
For our FacebookHandler we configure the use of Keyvault. For GoogleHandler we don't add anything.

When you debug both applications you'll notice a difference in the number of providers for the IConfiguration instance which enters the ServiceBusSubscriptionService.
For FacebookHandler there are 6 registered providers but for the GoogleHandler there are only 5.
The FacebookHandler has 1 additional provider: AzureKeyVaultConfigurationProvider

## Which provider has priority?
The last one in the list of providers wins. In our scenario, for the FacebookHandler, this means that the values in Azure Keyvault has priority over the value put in the appsettings.json file.

## How does the permission work, how does the console application access the keyvault?
In the Environment variables (properties of csproj) we configured the client id & secret. The keys are documented by Microsoft. 