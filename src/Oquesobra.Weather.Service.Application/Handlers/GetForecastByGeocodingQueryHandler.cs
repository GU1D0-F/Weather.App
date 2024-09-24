using MediatR;
using Oquesobra.Weather.Service.Cache;
using Oquesobra.Weather.Service.Domain.Commons;
using Oquesobra.Weather.Service.Infra.ExternalServices;
using Oquesobra.Weather.Service.Weather;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Oquesobra.Weather.Service.Application
{
    internal class GetForecastByGeocodingQueryHandler(IWeatherService weatherService, ICacheService cacheService) : Command, IRequestHandler<GetForecastByGeocodingQuery, WeatherProperties>
    {
        private readonly IWeatherService _weatherService = weatherService;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<WeatherProperties> Handle(GetForecastByGeocodingQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"WeatherData:{request.Lat},{request.Lng}";
            TimeSpan cacheDuration = TimeSpan.FromHours(1);

            try
            {
                return await _cacheService.GetOrSetAsync(cacheKey, async () =>
                {
                    var gridsResult = await _weatherService.GetGridsByLatLngAsync(Math.Round(request.Lat, 4), Math.Round(request.Lng, 4)) ?? throw new WeatherForecastException($"No grid data found for coordinates: {request.Lat}, {request.Lng}");

                    var weatherResult = await _weatherService.GetWeatherByGridsAsync(gridsResult.Properties.GridId, gridsResult.Properties.GridX, gridsResult.Properties.GridY);

                    if (weatherResult?.Properties == null)
                        throw new WeatherForecastException("No weather properties found for the given grid data.");

                    return weatherResult.Properties; //Aqui eu daria a sugestao de nao salvar o objeto inteiro em cache dado q e um objeto grande
                }, cacheDuration);
            }
            catch (WeatherForecastException ex)
            {
                Log.Error(ex, "Error retrieving weather forecast for coordinates: {Lat}, {Lng}", request.Lat, request.Lng);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while retrieving weather forecast for coordinates: {Lat}, {Lng}", request.Lat, request.Lng);
                throw new WeatherForecastException("An unexpected error occurred while processing your request.", ex);
            }
        }
    }
}
