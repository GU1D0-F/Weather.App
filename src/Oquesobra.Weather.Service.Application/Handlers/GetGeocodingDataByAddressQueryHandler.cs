using MediatR;
using Oquesobra.Weather.Service.Cache;
using Oquesobra.Weather.Service.Domain.Commons;
using Oquesobra.Weather.Service.Geocoding;
using Oquesobra.Weather.Service.Infra.GeocodingRest;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Oquesobra.Weather.Service.Application
{
    internal class GetGeocodingDataByAddressQueryHandler(IGeocodingService geocodingApi, ICacheService cacheService) : Command, IRequestHandler<GetGeocodingDataByAddressQuery, GeocodingCoordinates>
    {
        private readonly ICacheService _cacheService = cacheService;
        private readonly IGeocodingService _geocodingApi = geocodingApi;

        public async Task<GeocodingCoordinates> Handle(GetGeocodingDataByAddressQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"GeocodingData:{request.OneLineAddress}";
            TimeSpan cacheDuration = TimeSpan.FromHours(24);

            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                GeocodingByAddressResult geocodingRequest = await _geocodingApi.GetGeocodingDataFromAddressAsync(request.OneLineAddress);
                var addressMatch = geocodingRequest.Result.AddressMatches.FirstOrDefault();

                if (addressMatch != null)
                    return addressMatch.Coordinates;

                throw new Exception("No coordinates found for the given address.");
            }, cacheDuration);
        }
    }
}
