using System;
using System.Collections.Generic;
using System.Linq;
using Meteo.Models;

namespace Meteo.Infra
{

    public class MeteoService
    {
        private readonly List<WeatherRecord> _weatherRecords = new();

        

        public void AddWeatherRecord(WeatherRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            _weatherRecords.Add(record);
        }

        public bool UpdateWeatherRecord(WeatherRecord updatedRecord, int radiusMeters = 0)
        {
            if (radiusMeters < 0 || radiusMeters > 10000) // Limite de 10000 metros
                throw new ArgumentOutOfRangeException("Radio debe ser entre 1 y 10.000 metros.");

            if (updatedRecord == null) throw new ArgumentNullException(nameof(updatedRecord));

            var records = GetWeatherRecords(updatedRecord.Latitude, updatedRecord.Longitude, updatedRecord.DateTime, radiusMeters);

            if (!records.Any())
                throw new InvalidOperationException("No se encontraron registros para actualizar.");
                
            foreach (var record in records)
            {
                record.Humidity = updatedRecord.Humidity;
                record.WindSpeed = updatedRecord.WindSpeed;
                record.WindDirection = updatedRecord.WindDirection;
                record.Temperature = updatedRecord.Temperature;
                record.Pressure = updatedRecord.Pressure;
                record.Precipitation = updatedRecord.Precipitation;
                record.Radiation = updatedRecord.Radiation;
                record.Altitude = updatedRecord.Altitude;
            }
            return true;
        }

        public IEnumerable<WeatherRecord> GetWeatherRecords(double latitude, double longitude, DateTime? dateTime = null, int radiusMeters = 0)
        {
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException("Coordenadas inválidas.");

            if (radiusMeters < 0 || radiusMeters > 10000) // Limite de 1000 km
                throw new ArgumentOutOfRangeException("Radio debe ser entre 1 y 10.000 metros.");

            // Filter by coordinates and optional date
            var records = _weatherRecords.Where(r => r.Latitude == latitude && r.Longitude == longitude);

            if (dateTime.HasValue)
            {
                records = records.Where(r => r.DateTime.Date == dateTime.Value.Date);
            }

            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException("Coordenadas inválidas.");

            // Obtener registros dentro del radio especificado
            double EarthRadius = 6371000; // meters
            double latRad = latitude * Math.PI / 180.0;
            double lonRad = longitude * Math.PI / 180.0;

            records = _weatherRecords.Where(r =>
            {
                double dLat = (r.Latitude - latitude) * Math.PI / 180.0;
                double dLon = (r.Longitude - longitude) * Math.PI / 180.0;
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                            Math.Cos(latRad) * Math.Cos(r.Latitude * Math.PI / 180.0) *
                            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double distance = EarthRadius * c;
                return distance <= radiusMeters;
            });

            if (dateTime.HasValue)
            {
                records = records.Where(r => r.DateTime.Date == dateTime.Value.Date);
            }
            return records.ToList();
        }

        public Forecast GetForecast(double longitude, double latitude, int hours = 24)
        {
            if (longitude < -180 || longitude > 180 || latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException("Coordenadas inválidas.");

            if (hours <= 0 || hours > 168) // Limite de 7 days
                throw new ArgumentOutOfRangeException("Horas a simular entre 1 y 168");

            var records = CalculateForecast(longitude, latitude, hours);
            if (records == null || !records.Any())
                throw new InvalidOperationException("No se pudieron generar registros de pronóstico.");

            var forecast = new Forecast
            {
                AlgorithmVersion = "1.1-beta",
                Location = $"Lat: {latitude}, Lon: {longitude}",
                IsForecast = true,
                Records = records
            };

            return forecast;
        }

        private List<WeatherRecord> CalculateForecast(double longitude, double latitude, int hours)
        {
            var now = DateTime.UtcNow;
            var random = new Random();
            var simulatedRecords = new List<WeatherRecord>();

            for (int i = 0; i < hours; i++)
            {
                simulatedRecords.Add(new WeatherRecord
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    DateTime = now.AddHours(i),
                    Humidity = random.NextDouble() * 98, // 0-100%
                    WindSpeed = Math.Round(random.NextDouble() * 20, 2), // 0-20 m/s
                    WindDirection = Math.Round(random.NextDouble() * 360, 2) // 0-360°
                });
            }
            return simulatedRecords;
        }
        
    }
}
