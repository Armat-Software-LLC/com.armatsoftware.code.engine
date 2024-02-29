using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ArmatSoftware.Code.Engine.Storage.DI;

public static class CodeEngineStorageRegistration
{
    public static void UseCodeEngineStorage(this IServiceCollection services, IStorageAdapter storageAdapter = null)
    {
        services.AddScoped<IActionProvider, CodeEngineActionProvider>();
        services.AddScoped<IActionStorage, CodeEngineActionStorage>();

        if (storageAdapter != null)
        {
            services.AddScoped<IStorageAdapter>(provider => storageAdapter);
        }
    }
}