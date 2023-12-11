using FluentAssertions;
using Moq;
using System.Net;
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

    [Theory]
    [MemberData(nameof(ServiceTestsData))]
    public async Task GrabResourceFromURI_ReturnsValidString_WithDifferentApiResponses(string resourcePath, HttpStatusCode statusCode, string? expectedValue)
    {
        //Arrange
        _mockHttpService.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = statusCode is HttpStatusCode.OK ? new StringContent(expectedValue!) : null
                }
            );

        //Act
        var service = new WebScrapingService(_mockHttpService.Object);
        string result = await service.GrabResourceFromURIOrDefaultAsync(string.Empty, resourcePath);

        //Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedValue);
    }

    public static IEnumerable<object[]> ServiceTestsData()
    {
        yield return new object[] { "Resources/testCarsData.json", HttpStatusCode.OK, File.ReadAllText("Resources/testCarsData.json") }; // Successful API Response
        yield return new object[] { "Resources/testHTMLData.txt", HttpStatusCode.NotFound, File.ReadAllText("Resources/testHTMLData.txt") }; // Unsuccesful API Response
    }

    /*
    Seperate unit tests for both scenarios

    [Fact]
    public async Task GrabResourceFromURI_ReturnsValidString_WithSuccessfulApiResponse()
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
        var service = new WebScrapingService(_mockHttpService.Object);
        string result = await service.GrabResourceFromURIOrDefaultAsync(string.Empty, "Resources/testCarsData.json");

        //Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(expectedValue);
    }

    [Fact]
    public async Task GrabResourceFromURI_ReturnsValidString_WithUnsuccessfulApiResponse()
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
    */
}
