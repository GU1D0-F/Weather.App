using System.Collections.Generic;
using System.Net;

namespace Oquesobra.Weather.Service.Domain.Commons;

public class Result : IResult
{
    public Result(object value = null, bool isSuccess = true,
        HttpStatusCode responseCode = HttpStatusCode.OK)
    {
        Value = value;
        IsSuccess = isSuccess;
        ResponseCode = responseCode;
    }

    public bool HasValue => Value != null;
    public object Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public HttpStatusCode ResponseCode { get; }

    public T GetObjectValue<T>()
    {
        return (T)Value;
    }

    public static Result Ok(object value = null)
    {
        return new Result(value);
    }
}