using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebScraping.Controllers;
using WebScraping.Models;
using WebScraping.Services.Interfaces;

namespace Webscraping.Tests.ControllerTests
{
    public class HomeControllerTests
    {
        private readonly Mock<IWebScrapingService> _mockWebScrapingService;

        public HomeControllerTests()
        {
            _mockWebScrapingService = new Mock<IWebScrapingService>();
        }

        [Fact]
        public async Task Test_LoadScrapedInventory_Valid()
        {
            //Arrange
            var expectedResult = new List<CarViewModel>
            {
                new CarViewModel
                {
                    Odometer = Convert.ToString(11017),
                    Engine = "4 Cylinder",
                    FuelEconomy = Convert.ToString(22),
                    Transmission = "7-Speed Automatic",
                    Make = "Volkswagen",
                    Model = "Arteon",
                    ExteriorColor = "Oryx White Pearl Effect",
                    StockNumber = "30953P",
                    Trim = "2.0T SEL R-Line",
                    UUID = "3ffed5770a0e0a9478e8c686e9e53edf",
                    VIN = "WVWAR7AN4NE011039",
                    RetailValue = 42995.0,
                    AskingPrice = 38995.0,
                    BodyStyle = "AWD SEL R-Line 4Motion  Sedan",
                    ChromeId = 425649,
                    ModelCode = "3H82RT",
                    EngineSize = "2.0L",
                    FuelType = "Gasoline",
                    InternetPrice = "$38,995",
                    NewOrUsed = "used"
                }
            };
            string expectedHTMLString = File.ReadAllText("Resources/testHTMLData.txt");
            string expectedJsonString = File.ReadAllText("Resources/testCarsData.json");
            _mockWebScrapingService.SetupSequence(x => x.GrabResourceFromURIOrDefaultAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(
                expectedHTMLString
                )
                .ReturnsAsync(
                expectedJsonString
                );

            //Act
            var controller = new HomeController(_mockWebScrapingService.Object);
            var result = await controller.GetScrapedDataListAsync();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
