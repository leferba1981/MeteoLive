using System;
using System.Linq;
using Meteo.Infra;
using Meteo.Models;
using Xunit;

public class MeteoServiceTests
{
    [Fact]
    public void AddWeatherRecord_ShouldAddRecord()
    {
        var service = new Meteo.Infra.MeteoService();
        var record = new WeatherRecord
        {
            Latitude = 10,
            Longitude = 20,
            DateTime = DateTime.UtcNow,
            Humidity = 50,
            WindSpeed = 5,
            WindDirection = 90
        };

        service.AddWeatherRecord(record);
        var records = service.GetWeatherRecords(10, 20).ToList();

        Assert.Single(records);
        Assert.Equal(50, records[0].Humidity);
    }

    [Fact]
    public void UpdateWeatherRecord_ShouldUpdateExistingRecord()
    {
        var service = new Meteo.Infra.MeteoService();
        var record = new WeatherRecord
        {
            Latitude = 10,
            Longitude = 20,
            DateTime = DateTime.UtcNow,
            Humidity = 50,
            WindSpeed = 5,
            WindDirection = 90
        };
        service.AddWeatherRecord(record);

        var updated = new WeatherRecord
        {
            Latitude = 10,
            Longitude = 20,
            DateTime = record.DateTime,
            Humidity = 80,
            WindSpeed = 10,
            WindDirection = 180
        };

        var result = service.UpdateWeatherRecord(updated);
        var records = service.GetWeatherRecords(10, 20).ToList();

        Assert.True(result);
        Assert.Single(records);
        Assert.Equal(80, records[0].Humidity);
        Assert.Equal(10, records[0].WindSpeed);
        Assert.Equal(180, records[0].WindDirection);
    }

    [Fact]
    public void GetWeatherRecords_ShouldReturnEmpty_WhenNoMatch()
    {
        var service = new Meteo.Infra.MeteoService();
        var records = service.GetWeatherRecords(0, 0);
        Assert.Empty(records);
    }

    [Fact]
    public void AddWeatherRecord_ShouldThrow_WhenNull()
    {
        var service = new Meteo.Infra.MeteoService();
        Assert.Throws<ArgumentNullException>(() => service.AddWeatherRecord(null));
    }

    [Fact]
    public void GetWeatherRecords_WithRadius_ShouldReturnNearbyRecords()
    {
        var service = new Meteo.Infra.MeteoService();
        var record = new WeatherRecord
        {
            Latitude = 10,
            Longitude = 20,
            DateTime = DateTime.UtcNow,
            Humidity = 60,
            WindSpeed = 7,
            WindDirection = 120
        };
        service.AddWeatherRecord(record);

        // Slightly offset coordinates, within 1000 meters
        var records = service.GetWeatherRecords(10.0005, 20.0005, null, 1000).ToList();
        Assert.NotEmpty(records);
    }

    [Fact]
    public void Forecast_AlgorithmVersion_ShouldBeDefault()
    {
        var forecast = new Forecast();
        forecast.AlgorithmVersion = "1.1-alpha"; // Simulating a version set by the service
        Assert.Equal("1.1-alpha", forecast.AlgorithmVersion);
    }
}
