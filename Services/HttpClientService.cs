using WebScraping.Services.Interfaces;

namespace WebScraping.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            this._httpClient = httpClientFactory.CreateClient("CarsClient");
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _httpClient.GetAsync(url);
        }
    }
}
