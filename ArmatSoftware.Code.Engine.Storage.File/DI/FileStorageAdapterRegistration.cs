using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Storage.File.DI;

public static class FileStorageAdapterRegistration
{
    /// <summary>
    /// Add the default storage implementation of the IStorageAdapter to the service collection
    /// and use it for managing actions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="lifetime">
    /// Defaults to <see cref="ServiceLifetime.Scoped"/> to match typical per-request hosting (e.g. ASP.NET Core).
    /// Hosts without a request-scoped DI container (e.g. a long-lived language server resolving handlers from
    /// its root container) should pass <see cref="ServiceLifetime.Singleton"/> instead.
    /// </param>
    public static void UseCodeEngineFileAdapter(this IServiceCollection services, FileStorageOptions options, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.AddSingleton(options);

        // FileStorageAdapter takes a plain (non-generic) ILogger - type-based registration would
        // need the container to resolve ILogger directly, which isn't auto-registered by
        // Microsoft.Extensions.Logging (only ILogger<T> is). A factory delegate supplies an
        // ILogger<T> explicitly instead, which is always resolvable and is a valid ILogger.
        services.Add(new ServiceDescriptor(
            typeof(IStorageAdapter),
            sp => new FileStorageAdapter(options, sp.GetRequiredService<ILogger<FileStorageAdapter>>()),
            lifetime));
    }
}