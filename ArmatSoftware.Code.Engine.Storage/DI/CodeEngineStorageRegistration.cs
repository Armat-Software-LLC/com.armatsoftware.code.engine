using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Storage.DI;

public static class CodeEngineStorageRegistration
{
    /// <param name="services"></param>
    /// <param name="storageAdapter">Optional pre-built adapter instance to register directly instead of relying on a separately-registered <see cref="IStorageAdapter"/>.</param>
    /// <param name="lifetime">
    /// Defaults to <see cref="ServiceLifetime.Scoped"/> to match typical per-request hosting (e.g. ASP.NET Core).
    /// Hosts without a request-scoped DI container (e.g. a long-lived language server resolving handlers from
    /// its root container) should pass <see cref="ServiceLifetime.Singleton"/> instead.
    /// </param>
    public static void UseCodeEngineStorage(this IServiceCollection services, IStorageAdapter storageAdapter = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // CodeEngineActionProvider/CodeEngineActionStorage take a plain (non-generic) ILogger -
        // type-based registration would need the container to resolve ILogger directly, which
        // isn't auto-registered by Microsoft.Extensions.Logging (only ILogger<T> is). Factory
        // delegates supply an ILogger<T> explicitly instead, which is always resolvable and is a
        // valid ILogger.
        services.Add(new ServiceDescriptor(
            typeof(IActionProvider),
            sp => new CodeEngineActionProvider(sp.GetRequiredService<ILogger<CodeEngineActionProvider>>(), sp.GetRequiredService<IStorageAdapter>()),
            lifetime));
        services.Add(new ServiceDescriptor(
            typeof(IActionStorage),
            sp => new CodeEngineActionStorage(sp.GetRequiredService<ILogger<CodeEngineActionStorage>>(), sp.GetRequiredService<IStorageAdapter>()),
            lifetime));

        if (storageAdapter != null)
        {
            services.Add(new ServiceDescriptor(typeof(IStorageAdapter), _ => storageAdapter, lifetime));
        }
    }
}