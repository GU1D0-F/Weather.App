using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oquesobra.Weather.Service.Application;
using Oquesobra.Weather.Service.Infra.ExternalServices;
using Oquesobra.Weather.Service.Infra.GeocodingRest;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Network;

namespace Oquesobra.Weather.Service.Api;

/// <summary>
/// Configures services and the request pipeline for the application.
/// </summary>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the Startup class and builds the application configuration.
    /// </summary>
    /// <param name="env">Provides information about the web hosting environment.</param>
    public Startup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    /// <summary>
    /// Gets the application configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configures services for dependency injection and other application services.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddCache();

        services.AddSwagger(Configuration);

        services.AddHealthChecks();

        services.AddCors();

        services.AddApiClient<IGeocodingService>(Configuration["GeocodingApi:BaseUrl"]);

        services.AddApiClient<IWeatherService>(Configuration["WeatherApi:BaseUrl"]);

        services.AddMediatR(config => config.RegisterServicesFromAssemblies(typeof(ApplicationModule).Assembly));

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", Configuration.GetValue<string>("Serilog:applicationName"))
            .Enrich.WithExceptionDetails()
            .WriteTo.UDPSink(Configuration.GetValue<string>("Serilog:remoteAddress"),
                Configuration.GetValue<int>("Serilog:remotePort"))
            .MinimumLevel.Is(Serilog.Events.LogEventLevel.Information)
            .CreateLogger();
    }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">The application builder used to define middleware components.</param>
    /// <param name="env">Provides information about the web hosting environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(e =>
        {
            e.AllowAnyOrigin();
            e.AllowAnyMethod();
            e.AllowAnyHeader();
        });

        app.UseAuthorization();
        app.UseAuthentication();

        app.UseSwagger(Configuration);

        app.UseHealthChecks("/health");

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}