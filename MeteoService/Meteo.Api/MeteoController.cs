using Microsoft.AspNetCore.Mvc;
using Meteo.Infra;
using Meteo.Models;
using System.Threading.Tasks;

namespace MeteoLive.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly MeteoApiClient _meteoApiClient;

        public WeatherController(MeteoApiClient meteoApiClient)
        {
            _meteoApiClient = meteoApiClient;
        }

        [HttpGet("QueryLocation")]
        public async Task<IActionResult> QueryLocation([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var forecast = await _meteoApiClient.QueryLocationAsync(latitude, longitude);

            if (forecast == null)
                return StatusCode(502, "Error querying Open-Meteo API.");

            return Ok(forecast);
        }
    }
}