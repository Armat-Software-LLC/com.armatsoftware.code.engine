using ArmatSoftware.Code.Engine.Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ArmatSoftware.Code.Engine.Storage.DI;

public static class CodeEngineRepositoryRegistration
{
    public static void UseCodeEngineDefaultRepository(this IServiceCollection services)
    {
        services.AddScoped<IActionRepository, CodeEngineActionRepository>();
    }
    
    public static void UseCodeEngineRepository<TStorage>(this IServiceCollection services)
    {
        services.AddScoped(typeof(IActionRepository), typeof(TStorage));
    }
    
    public static void UseCodeEngineRepository(this IServiceCollection services, IActionRepository repository)
    {
        services.AddScoped(provider => repository);
    }
}