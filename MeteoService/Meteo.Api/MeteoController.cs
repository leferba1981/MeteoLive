using Microsoft.AspNetCore.Mvc;
using Meteo.Infra;
using Meteo.Models;
using System.Threading.Tasks;

namespace Meteo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeteoController : ControllerBase
    {
        private readonly MeteoApiClient _meteoApiClient;

        private readonly MeteoService meteoService;

        public MeteoController(MeteoService _meteoService, HttpClient httpClient)
        {
            meteoService = _meteoService;
            _meteoApiClient = new MeteoApiClient(httpClient);
        }

        /// <summary>
        /// Almacenar un nuevo registro meteorológico
        /// </summary>
        [HttpPost("WeatherRecord")]
        public IActionResult RegisterWeatherRecord([FromBody] WeatherRecord record)
        {
            if (record == null)
                return BadRequest("Invalid record.");
            meteoService.AddWeatherRecord(record);
            return Ok(new { message = "Weather record registered successfully." });
        }

        /// <summary>
        /// Actualiza un registro meteorológico existente.
        /// </summary>
        [HttpPut("WeatherRecord")]
        public IActionResult UpdateWeatherRecord([FromBody] WeatherRecord record, [FromQuery] int radiusMeters = 0)
        {
            if (record == null)
                return BadRequest("Invalid record.");

            try
            {
                var updated = meteoService.UpdateWeatherRecord(record, radiusMeters);
                if (!updated)
                    return NotFound(new { message = "No se encontraron registros para actualizar." });

                return Ok(new { message = "Weather record updated successfully." });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un pronóstico simulado para una ubicación y cantidad de horas.
        /// </summary>
        [HttpGet("Forecast")]
        public IActionResult GetForecast([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] int hours = 24)
        {
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                return BadRequest("Coordenadas inválidas.");
            if (hours <= 0 || hours > 168)
                return BadRequest("El número de horas debe estar entre 1 y 168.");

            var forecast = meteoService.GetForecast(longitude, latitude, hours);
            return Ok(forecast);
        }

        /// <summary>
        /// Obtiene datos históricos meteorológicos para una ubicación, fecha y radio.
        /// </summary>
        [HttpGet("Historical")]
        public IActionResult GetHistorical(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] string? date = null,
            [FromQuery] int radiusMeters = 0)
        {
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                return BadRequest("Coordenadas inválidas.");

            DateTime? dateTime = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (!DateTime.TryParse(date, out var parsedDate))
                    return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");
                dateTime = parsedDate;
            }

            var records = meteoService.GetWeatherRecords(latitude, longitude, dateTime, radiusMeters);
            if (records == null || !records.Any())
                return NotFound(new { message = "No se encontraron registros históricos para la ubicación y fecha especificadas." });
            var forecast = new Forecast
            {
                Location = $"{latitude},{longitude}",
                IsForecast = false,
                Records = records.ToList()
            };
            return Ok(forecast);
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