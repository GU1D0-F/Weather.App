using System.Net;

namespace Oquesobra.Weather.Service.Domain.Commons;

public interface IResult
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    bool HasValue { get; }

    HttpStatusCode ResponseCode { get; }

    T GetObjectValue<T>();
}