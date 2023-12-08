using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WebScraping.Models;
using WebScraping.Services.Interfaces;

namespace WebScraping.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebScrapingService _webScrapingService;

        public HomeController(IWebScrapingService webScrapingService)
        {
            this._webScrapingService = webScrapingService;
        }

        public async Task<IActionResult> LoadScrapedInventory()
        {
            return View("Index", await GetScrapedDataListAsync());
        }

        public async Task<List<CarViewModel>> GetScrapedDataListAsync()
        {
            //Retrieving and loading the source HTML as a string
            var htmlString = await _webScrapingService.GrabResourceFromURIOrDefaultAsync(@"/used-inventory/index.htm", @"BackupResource\html.txt");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);

            //Extracting inventory API URL using XPath and String Manipulation
            var scriptNode = htmlDoc.DocumentNode.SelectSingleNode("//script[contains(., 'inventoryApiURL')]");
            string inventoryApiUrl = string.Empty;
            if (scriptNode != null)
            {
                //URI can be found in a script tag. Inside it, the URI is in between the keywords "inventoryApiURL" & "spellcheckApiURL"
                var scriptContent = scriptNode.InnerText;
                int startIndex = scriptContent.IndexOf("\"inventoryApiURL\":") + "'inventoryApiURL':".Length;
                int endIndex = scriptContent.IndexOf("\"spellcheckApiURL\"", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    inventoryApiUrl = scriptContent[startIndex..endIndex].Trim(' ', '\'', '"', ',', '\n');
                }
            }

            //Obtaining the used vehicle data from the extracted inventory URL
            var responseParentJson = await _webScrapingService.GrabResourceFromURIOrDefaultAsync(inventoryApiUrl, @"BackupResource\usedCarsData.json");

            // Querying the vehicle data json to get the trackingData array
            var trackingData = JArray.Parse(Convert.ToString(JObject.Parse(responseParentJson)?.SelectToken("pageInfo.trackingData"))!);

            return trackingData.Select(x => new CarViewModel
            {
                Odometer = x.SelectToken("odometer")?.ToString(),
                Engine = x.SelectToken("engine")?.ToString(),
                FuelEconomy = x.SelectToken("cityFuelEfficiency")?.ToString(),
                Transmission = x.SelectToken("transmission")?.ToString(),
                Make = x.SelectToken("make")?.ToString(),
                Model = x.SelectToken("model")?.ToString(),
                ExteriorColor = x.SelectToken("exteriorColor")?.ToString(),
                StockNumber = x.SelectToken("stockNumber")?.ToString(),
                Trim = x.SelectToken("trim")?.ToString(),
                UUID = x.SelectToken("uuid")?.ToString(),
                VIN = x.SelectToken("vin")?.ToString(),
                RetailValue = Double.Parse(x.SelectToken("retailValue")?.ToString() ?? "0"),
                AskingPrice = Double.Parse(x.SelectToken("askingPrice")?.ToString() ?? "0"),
                BodyStyle = x.SelectToken("bodyStyle")?.ToString(),
                ChromeId = int.Parse(x.SelectToken("chromeId")?.ToString() ?? "0"),
                ModelCode = x.SelectToken("modelCode")?.ToString(),
                EngineSize = x.SelectToken("engineSize")?.ToString(),
                FuelType = x.SelectToken("fuelType")?.ToString(),
                InternetPrice = x.SelectToken("internetPrice")?.ToString(),
                NewOrUsed = x.SelectToken("newOrUsed")?.ToString()
            }).ToList();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
