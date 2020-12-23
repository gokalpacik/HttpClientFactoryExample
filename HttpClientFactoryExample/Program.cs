using HttpClientFactoryExample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientFactoryExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            try
            {
                await serviceProvider.GetService<IIntegrationService>().Run();
            }
            catch (Exception generalException)
            {
                var logger = serviceProvider.GetService<ILogger<Program>>();
                logger.LogError(generalException, "An exception happened while running the integration service.");
            }

            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(builder => builder.AddConsole());


            serviceCollection.AddHttpClient("CocktailClient", client =>
            {
                client.BaseAddress = new Uri("https://www.thecocktaildb.com/");
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Clear();
            }).ConfigurePrimaryHttpMessageHandler(handler =>
            new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            });

            serviceCollection.AddScoped<IIntegrationService, HttpClientFactoryInstanceManagementService>();
        }
    }
}