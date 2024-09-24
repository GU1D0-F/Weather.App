using Microsoft.AspNetCore.Mvc;
using Oquesobra.Weather.Service.Domain.Commons;

namespace Oquesobra.Weather.Service.Api;


/// <summary>
/// A base controller that provides a common result handling mechanism for all controllers.
/// </summary>
[ApiController]
public class BaseController : ControllerBase
{
    /// <summary>
    /// Returns an IActionResult based on the IResult passed, handling success or failure status codes.
    /// </summary>
    /// <param name="result">The result object that contains the response code and data.</param>
    /// <returns>IActionResult with the appropriate HTTP status code and response content.</returns>
    protected IActionResult AsResult(IResult result)
    {
        return result.IsFailure
            ? StatusCode((int)result.ResponseCode)
            : StatusCode((int)result.ResponseCode, result.GetObjectValue<object>());
    }
}