using System.Reflection;

namespace WebScraping.Services.Interfaces
{
    public interface IWebScrapingService
    {
        Task<string> GrabResourceFromURIOrDefaultAsync(string Uri, string backupResourceRelativePath);
    }
}
