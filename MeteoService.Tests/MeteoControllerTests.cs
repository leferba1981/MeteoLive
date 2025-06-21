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
}
