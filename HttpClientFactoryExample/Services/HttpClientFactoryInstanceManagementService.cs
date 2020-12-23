using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using HttpClientFactoryExample.Extensions;
using HttpClientFactoryExample.Model;
using Microsoft.Extensions.Logging;

namespace HttpClientFactoryExample.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientFactoryInstanceManagementService> _logger;

        public HttpClientFactoryInstanceManagementService(
            IHttpClientFactory httpClientFactory,
            ILogger<HttpClientFactoryInstanceManagementService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Run()
        {
            await GetDrinksWithHttpClientFromFactory(_cancellationTokenSource.Token);
        }

        private async Task GetDrinksWithHttpClientFromFactory(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("CocktailClient");

            var request = new HttpRequestMessage(HttpMethod.Get, "api/json/v1/1/random.php");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var stream = await response.Content.ReadAsStreamAsync();
            response.EnsureSuccessStatusCode();
            var drinks = stream.ReadAndDeserializeFromJson<DrinkResult>();
            _logger.LogInformation(drinks.Drinks.FirstOrDefault()?.StrAlcoholic);
        }
    }
}
