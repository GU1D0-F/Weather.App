using Oquesobra.Weather.Service.Geocoding;
using Refit;
using System.Threading.Tasks;

namespace Oquesobra.Weather.Service.Infra.GeocodingRest;

public interface IGeocodingService
{
    [Get("/geocoder/locations/onelineaddress")]
    Task<GeocodingByAddressResult> GetGeocodingDataFromAddressAsync([Query] string address,
        [Query] string benchmark = "2020", [Query] string format = "json");
}