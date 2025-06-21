using Xunit;
using Meteo.Api.Controllers;
using Meteo.Infra;
using Meteo.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class MeteoControllerTests
{
    [Fact]
    public async Task QueryLocation_ReturnsOk_WhenForecastIsNotNull()
    {
        var hourlyUnits = new HourlyUnits("°C", "°C", "mm", "km/h", "mm", "yes");
        var hourlyData = new HourlyData(
            new List<long> { 1633035600 },
            new List<double> { 20.5 },
            new List<double> { 19.5 },
            new List<double> { 0.0 },
            new List<double> { 5.0 },
            new List<double> { 0.0 }
        );

        var mockApiClient = new Mock<MeteoApiClient>();
        mockApiClient.Setup(x => x.QueryLocationAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new Forecast(4, -71, 1000, 3600, "America/New_York", "EST", 10, hourlyUnits, hourlyData));

        var controller = new MeteoController(mockApiClient.Object);

        // Act
        var result = await controller.QueryLocation(4, -71);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task QueryLocation_Returns502_WhenForecastIsNull()
    {
        // Arrange
        var mockApiClient = new Mock<MeteoApiClient>();
        mockApiClient.Setup(x => x.QueryLocationAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((Forecast)null);

        var controller = new MeteoController(mockApiClient.Object);

        // Act
        var result = await controller.QueryLocation(4, -390);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(502, statusResult.StatusCode);
    }
}
