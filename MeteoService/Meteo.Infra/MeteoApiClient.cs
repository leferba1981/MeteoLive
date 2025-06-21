using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Meteo.Models;

namespace Meteo.Infra
{
    public class MeteoApiClient
    {
        private readonly HttpClient _httpClient;

        public MeteoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Forecast?> QueryLocationAsync(double latitude, double longitude)
        {
            var latitudeStr = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var longitudeStr = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitudeStr}&longitude={longitudeStr}&hourly=temperature_2m,apparent_temperature,precipitation,wind_speed_10m,rain&timezone=auto&forecast_days=1&format=json&timeformat=unixtime";
            Console.WriteLine($"Querying Open-Meteo API: {url}");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Forecast>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

    }
}