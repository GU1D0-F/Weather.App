using MediatR;
using Oquesobra.Weather.Service.Domain.Commons;

namespace Oquesobra.Weather.Service.Geocoding
{
    public class GetGeocodingDataByAddressQuery : Query, IRequest<GeocodingCoordinates>
    {
        public string OneLineAddress { get; set; }
    }
}
