using Bogus;
using Moq;
using Oquesobra.Weather.Service.Application;
using Oquesobra.Weather.Service.Cache;
using Oquesobra.Weather.Service.Geocoding;
using Oquesobra.Weather.Service.Infra.GeocodingRest;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Oquesobra.Weather.Service.UnitTests
{
    public class GetGeocodingDataByAddressQueryHandlerTests
    {
        private readonly Mock<IGeocodingService> _mockGeocodingService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly GetGeocodingDataByAddressQueryHandler _handler;
        private readonly Faker _faker;

        public GetGeocodingDataByAddressQueryHandlerTests()
        {
            _mockGeocodingService = new Mock<IGeocodingService>();
            _mockCacheService = new Mock<ICacheService>();
            _handler = new GetGeocodingDataByAddressQueryHandler(_mockGeocodingService.Object, _mockCacheService.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_ShouldReturnCoordinates_FromCache_WhenAvailable()
        {
            // Arrange
            var address = _faker.Address.FullAddress();
            var expectedCoordinates = new GeocodingCoordinates { X = _faker.Random.Double(), Y = _faker.Random.Double() };
            var cacheKey = $"GeocodingData:{address}";

            _mockCacheService.Setup(x => x.GetOrSetAsync(
                cacheKey,
                It.IsAny<Func<Task<GeocodingCoordinates>>>(),
                It.IsAny<TimeSpan>())
            ).ReturnsAsync(expectedCoordinates);

            var request = new GetGeocodingDataByAddressQuery { OneLineAddress = address };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(expectedCoordinates, result);


            _mockGeocodingService.Verify(x => x.GetGeocodingDataFromAddressAsync(address, "2020", "json"), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallApi_AndCacheResult_WhenNotInCache()
        {
            // Arrange
            var address = _faker.Address.FullAddress();
            var expectedCoordinates = new GeocodingCoordinates { X = _faker.Random.Double(), Y = _faker.Random.Double() };
            var cacheKey = $"GeocodingData:{address}";

            _mockCacheService.Setup(x => x.GetOrSetAsync(
                cacheKey,
                It.IsAny<Func<Task<GeocodingCoordinates>>>(),
                It.IsAny<TimeSpan>())
            ).ReturnsAsync((string key, Func<Task<GeocodingCoordinates>> getItemCallback, TimeSpan cacheDuration) =>
            {
                return getItemCallback().Result;
            });
            
            _mockGeocodingService.Setup(x => x.GetGeocodingDataFromAddressAsync(address, "2020", "json"))
                .ReturnsAsync(new GeocodingByAddressResult
                {
                    Result = new GeocodingAddressResult
                    {
                        AddressMatches = [new GeocodingAddress { Coordinates = expectedCoordinates }]
                    }
                });

            var request = new GetGeocodingDataByAddressQuery { OneLineAddress = address };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(expectedCoordinates, result);

            _mockGeocodingService.Verify(x => x.GetGeocodingDataFromAddressAsync(address, "2020", "json"), Times.Once);

            _mockCacheService.Verify(x => x.GetOrSetAsync(
                cacheKey,
                It.IsAny<Func<Task<GeocodingCoordinates>>>(),
                It.IsAny<TimeSpan>()), Times.Once);
        }

    }
}
