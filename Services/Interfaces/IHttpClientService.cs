namespace WebScraping.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}
