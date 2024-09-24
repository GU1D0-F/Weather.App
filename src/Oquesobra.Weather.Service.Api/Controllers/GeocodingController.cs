using MediatR;
using Microsoft.AspNetCore.Mvc;
using Oquesobra.Weather.Service.Application;
using Oquesobra.Weather.Service.Domain.Commons;
using Oquesobra.Weather.Service.Geocoding;
using System.Threading.Tasks;

namespace Oquesobra.Weather.Service.Api;

/// <summary>
/// Controller responsible for handling geocoding-related API endpoints.
/// </summary>
[Route("api/v1/geocoding")]
[ApiController]
public class GeocodingController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves geocoding information.
    /// </summary>
    /// <returns>An asynchronous task that returns geocoding data.</returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string oneLineAddress)
    {
        var coordinates = await _mediator.Send(new GetGeocodingDataByAddressQuery() { OneLineAddress = oneLineAddress });

        return AsResult(Result.Ok(new GetGeocodingDataByAddressResponse(coordinates.X, coordinates.Y)));
    }
}