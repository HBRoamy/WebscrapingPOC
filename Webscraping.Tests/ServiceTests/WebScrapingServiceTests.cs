using FluentAssertions;
using Moq;
using WebScraping.Services;
using WebScraping.Services.Interfaces;

namespace Webscraping.Tests.ServiceTests;

public class WebScrapingServiceTests
{
    private readonly Mock<IHttpClientService> _mockHttpService;
    public WebScrapingServiceTests()
    {
        _mockHttpService = new Mock<IHttpClientService>();
    }

    [Fact]
    public async Task Test_GrabResourceFromURI_ReturnsValidString_WithSuccessfulApiResponse()
    {
        //Arrange
        string expectedValue = File.ReadAllText("Resources/testCarsData.json");
        _mockHttpService.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(
            new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(expectedValue)
            }
            );

        //Act
        var service = new WebScrapingService( _mockHttpService.Object );
        string result = await service.GrabResourceFromURIOrDefaultAsync(string.Empty, "Resources/testCarsData.json");

        //Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedValue);
    }

    [Fact]
    public async Task Test_GrabResourceFromURI_ReturnsValidString_WithUnsuccessfulApiResponse()
    {
        //Arrange
        string expectedValue = File.ReadAllText("Resources/testHTMLData.txt");
        _mockHttpService.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(
            new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            }
            );

        //Act
        var service = new WebScrapingService(_mockHttpService.Object);
        string result = await service.GrabResourceFromURIOrDefaultAsync(string.Empty, "Resources/testHTMLData.txt");

        //Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedValue);
    }
}
