using ArmatSoftware.Code.Engine.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ArmatSoftware.Code.Engine.Storage.File.DI;

public static class CodeEngineFileStorageRegistration
{
    /// <summary>
    /// Add the file storage implementation of the IActionRepository to the service collection
    /// and use it for managing actions. This is a necessary registration for IActionRepository injection
    /// </summary>
    /// <param name="services"></param>
    public static void UseCodeEngineFileStorage(this IServiceCollection services)
    {
        services.AddScoped<IActionRepository, CodeEngineActionRepository>();
    }
}