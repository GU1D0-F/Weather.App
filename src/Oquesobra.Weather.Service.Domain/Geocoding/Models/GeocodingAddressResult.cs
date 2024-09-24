using System.Collections.Generic;

namespace Oquesobra.Weather.Service.Geocoding;

public class GeocodingAddressResult
{
    public IEnumerable<GeocodingAddress> AddressMatches { get; set; }
}