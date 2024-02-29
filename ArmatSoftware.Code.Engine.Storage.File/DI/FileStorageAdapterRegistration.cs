using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ArmatSoftware.Code.Engine.Storage.File.DI;

public static class FileStorageAdapterRegistration
{
    /// <summary>
    /// Add the default storage implementation of the IStorageAdapter to the service collection
    /// and use it for managing actions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    public static void UseCodeEngineFileAdapter(this IServiceCollection services, FileStorageOptions options)
    {
        services.AddSingleton(options);
        services.AddScoped<IStorageAdapter, FileStorageAdapter>();
    }
}