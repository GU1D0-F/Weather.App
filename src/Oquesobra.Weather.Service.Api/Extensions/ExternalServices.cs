using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Refit;
using System;
using System.Net.Http;

namespace Oquesobra.Weather.Service.Api;

/// <summary>
/// Extension methods for adding external service clients to the dependency injection container.
/// </summary>
public static class ExternalServices
{
    /// <summary>
    /// Registers an API client using Refit with a specified base URL.
    /// </summary>
    /// <typeparam name="T">The API interface type.</typeparam>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="urlBase">The base URL for the API client.</param>
    public static void AddApiClient<T>(this IServiceCollection services, string urlBase) where T : class
    {
        IHttpClientBuilder refitClientBuilder =
            services.AddRefitClient<T>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(urlBase);
                })
                .AddPolicyHandler(GetRetryPolicy());
    }

    /// <summary>
    /// Configures a retry policy for HTTP requests using Polly.
    /// This policy retries the request once after a delay of 20 seconds in case of transient errors, 
    /// such as network failures or server errors (5xx, 408).
    /// </summary>
    /// <returns>
    /// An asynchronous policy that handles transient HTTP errors and applies a single retry 
    /// after a 20-second delay.
    /// </returns>
    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(1, _ => TimeSpan.FromSeconds(20));
    }
}