using System.Collections.Generic;

namespace Oquesobra.Weather.Service.Weather;

public class WeatherProperties
{
    public string GridId { get; set; }
    public int GridX { get; set; }
    public int GridY { get; set; }
    public IEnumerable<WeatherPeriod> Periods { get; set; }
    public ElevationArea Elevation { get; set; }
}