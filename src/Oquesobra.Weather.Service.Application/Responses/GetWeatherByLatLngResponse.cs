using Oquesobra.Weather.Service.Weather;
using System;
using System.Collections.Generic;

namespace Oquesobra.Weather.Service.Application;

public class GetWeatherByAddressResponse(IEnumerable<WeatherPeriod> periods, ElevationArea elevation)
{
    public IEnumerable<WeatherPeriod> Periods { get; set; } = periods;
    public ElevationArea Elevation { get; set; } = elevation;
}


public static class WeatherMapper
{
    public static GetWeatherByAddressResponse MapToGetWeatherByAddressResponse(WeatherProperties weatherProperties)
    {
        if (weatherProperties == null)
            throw new ArgumentNullException(nameof(weatherProperties), "WeatherProperties cannot be null");

        return new GetWeatherByAddressResponse(
            weatherProperties.Periods,
            weatherProperties.Elevation
        );
    }
}