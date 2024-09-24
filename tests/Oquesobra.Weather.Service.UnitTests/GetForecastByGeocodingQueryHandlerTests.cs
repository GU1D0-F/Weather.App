using Bogus;
using Moq;
using Oquesobra.Weather.Service.Application;
using Oquesobra.Weather.Service.Cache;
using Oquesobra.Weather.Service.Infra.ExternalServices;
using Oquesobra.Weather.Service.Weather;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Oquesobra.Weather.Service.UnitTests
{
    public class GetForecastByGeocodingQueryHandlerTests
    {
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly GetForecastByGeocodingQueryHandler _handler;
        private readonly Faker _faker;

        public GetForecastByGeocodingQueryHandlerTests()
        {
            _weatherServiceMock = new Mock<IWeatherService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new GetForecastByGeocodingQueryHandler(_weatherServiceMock.Object, _cacheServiceMock.Object);
            _faker = new Faker();
        }

        private GridsResult GenerateGridsResult()
        {
            return new GridsResult(
                _faker.Random.AlphaNumeric(10),
                new WeatherProperties
                {
                    GridId = _faker.Random.AlphaNumeric(10),
                    GridX = _faker.Random.Int(0, 100),
                    GridY = _faker.Random.Int(0, 100),
                    Periods =
                    [
                        new WeatherPeriod
                        {
                            Name = _faker.Lorem.Word(),
                            StartTime = _faker.Date.Future(),
                            EndTime = _faker.Date.Future(),
                            Temperature = _faker.Random.Long(-30, 50),
                        }
                    ],
                    Elevation = new ElevationArea
                    {
                        UnitCode = "m",
                        Value = _faker.Random.Double(0, 3000)
                    }
                }
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnWeatherProperties_WhenDataIsFound()
        {
            // Arrange
            var query = new GetForecastByGeocodingQuery(_faker.Random.Double(-90, 90), _faker.Random.Double(-180, 180));
            var gridsResult = GenerateGridsResult();

            var expectedWeatherProperties = new WeatherProperties
            {
                GridId = gridsResult.Properties.GridId,
                GridX = gridsResult.Properties.GridX,
                GridY = gridsResult.Properties.GridY,
                Periods = gridsResult.Properties.Periods,
                Elevation = gridsResult.Properties.Elevation
            };

            _cacheServiceMock
                .Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<WeatherProperties>>>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(expectedWeatherProperties);

            _weatherServiceMock
                .Setup(x => x.GetGridsByLatLngAsync(query.Lat, query.Lng))
                .ReturnsAsync(gridsResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedWeatherProperties.GridId, result.GridId);
            Assert.Equal(expectedWeatherProperties.GridX, result.GridX);
            Assert.Equal(expectedWeatherProperties.GridY, result.GridY);
            Assert.Equal(expectedWeatherProperties.Periods, result.Periods);
            Assert.Equal(expectedWeatherProperties.Elevation, result.Elevation);
        }

        [Fact]
        public async Task Handle_ShouldThrowWeatherForecastException_WhenNoWeatherPropertiesFound()
        {
            // Arrange
            var query = new GetForecastByGeocodingQuery(_faker.Random.Double(-90, 90), _faker.Random.Double(-180, 180));

            _cacheServiceMock
               .Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<WeatherProperties>>>(), It.IsAny<TimeSpan>()))
               .ThrowsAsync(new WeatherForecastException("No weather properties found for the given grid data."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<WeatherForecastException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("No weather properties found for the given grid data.", exception.Message);
        }


        [Fact]
        public async Task Handle_ShouldThrowWeatherForecastException_WhenNoGridDataFound()
        {
            // Arrange
            var query = new GetForecastByGeocodingQuery(_faker.Random.Double(-90, 90), _faker.Random.Double(-180, 180));

            _cacheServiceMock
                .Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<WeatherProperties>>>(), It.IsAny<TimeSpan>()))
                .ThrowsAsync(new WeatherForecastException("No grid data found."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<WeatherForecastException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("No grid data found.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var query = new GetForecastByGeocodingQuery(_faker.Random.Double(-90, 90), _faker.Random.Double(-180, 180));

            _cacheServiceMock
                .Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<WeatherProperties>>>(), It.IsAny<TimeSpan>()))
                .ThrowsAsync(new Exception("Unexpected error."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<WeatherForecastException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Contains("An unexpected error occurred while processing your request.", exception.Message);
        }
    }
}
