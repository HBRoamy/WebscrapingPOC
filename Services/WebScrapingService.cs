using System.Net;
using WebScraping.Services.Interfaces;

namespace WebScraping.Services;

public class WebScrapingService : IWebScrapingService
{
    private readonly IHttpClientService httpClientService;

    public WebScrapingService(IHttpClientService httpClientService)
    {
        this.httpClientService = httpClientService;
    }
    public async Task<string> GrabResourceFromURIOrDefaultAsync(string Uri, string backupResourceRelativePath)
    {
        var response = await httpClientService.GetAsync(Uri);

        var responseString = string.Empty;
        if(response.StatusCode.Equals(HttpStatusCode.OK))
        {
            responseString = await response.Content.ReadAsStringAsync();
        }

        if(string.IsNullOrWhiteSpace(responseString))
        {
            responseString = File.ReadAllText(backupResourceRelativePath); ;
        }

        return responseString!;
    }
}

