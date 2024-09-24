using MediatR;
using Microsoft.AspNetCore.Mvc;
using Oquesobra.Weather.Service.Application;
using Oquesobra.Weather.Service.Domain.Commons;
using Oquesobra.Weather.Service.Geocoding;
using Oquesobra.Weather.Service.Weather;
using System.Threading.Tasks;

namespace Oquesobra.Weather.Service.Api;

/// <summary>
/// Controller responsible for handling weather-related API endpoints.
/// </summary>
[Route("api/v1/weather")]
[ApiController]
public class WeatherController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves weather information.
    /// </summary>
    /// <returns>An asynchronous task that returns weather data.</returns>
    [HttpGet("{lat},{lng}")]
    public async Task<IActionResult> Get(double lat, double lng)
    {
        var weatherProperties = await _mediator.Send(new GetForecastByGeocodingQuery(lng, lat));

        return AsResult(Result.Ok(WeatherMapper.MapToGetWeatherByAddressResponse(weatherProperties)));
    }

    /// <summary>
    /// Retrieves the weather forecast based on the provided one-line address.
    /// </summary>
    /// <param name="oneLineAddress">The address provided as a single line of text.</param>
    /// <returns>An IActionResult object containing the mapped weather forecast response for the address.</returns>
    [HttpGet("GetByAddressLine")]
    public async Task<IActionResult> GetByAddressLine([FromQuery] string oneLineAddress)
    {
        var coordinates = await _mediator.Send(new GetGeocodingDataByAddressQuery() { OneLineAddress = oneLineAddress });

        var weatherProperties = await _mediator.Send(new GetForecastByGeocodingQuery(coordinates.X, coordinates.Y));

        return AsResult(Result.Ok(WeatherMapper.MapToGetWeatherByAddressResponse(weatherProperties)));
    }
}