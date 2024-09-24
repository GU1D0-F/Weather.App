using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Oquesobra.Weather.Service.Api;

/// <summary>
/// Extension methods for configuring Swagger for API documentation.
/// </summary>
public static class Swagger
{
    /// <summary>
    /// Adds Swagger to the service collection with the provided configuration.
    /// </summary>
    /// <param name="serviceCollection">The IServiceCollection instance.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void AddSwagger(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"API | {configuration["Swagger:Title"]}",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Email = "tecnologia@oquesobra.com.br",
                    Name = "Oquesobra - Engenharia de Software"
                },
                Description = $"{configuration["Swagger:Description"]}"
            });
        });
    }


    /// <summary>
    /// Configures the application to use Swagger and Swagger UI for API documentation.
    /// </summary>
    /// <param name="applicationBuilder">The IApplicationBuilder instance.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void UseSwagger(this IApplicationBuilder applicationBuilder, IConfiguration configuration)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"API - {configuration["Swagger:Title"]}");
            c.RoutePrefix = string.Empty;
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });
    }
}