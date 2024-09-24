using MediatR;
using Oquesobra.Weather.Service.Domain.Commons;

namespace Oquesobra.Weather.Service.Weather
{
    public class GetForecastByGeocodingQuery(double x, double y) : Query, IRequest<WeatherProperties>
    {
        public double Lat { get; set; } = y;
        public double Lng { get; set; } = x;
    }
}
