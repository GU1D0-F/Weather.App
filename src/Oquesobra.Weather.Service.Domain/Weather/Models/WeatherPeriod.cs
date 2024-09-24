using System;

namespace Oquesobra.Weather.Service.Weather;

public class WeatherPeriod
{
    public long Number { get; set; }
    public string Name { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public bool IsDaytime { get; set; }
    public long Temperature { get; set; }
    public TemperatureUnit TemperatureUnit { get; set; }
    public object TemperatureTrend { get; set; }
    public string WindSpeed { get; set; }
    public string WindDirection { get; set; }
    public Uri Icon { get; set; }
    public string ShortForecast { get; set; }
    public string DetailedForecast { get; set; }
}

public enum TemperatureUnit
{
    F
};