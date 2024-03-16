using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArmatSoftware.Code.Engine.Compiler.Execution;
using ArmatSoftware.Code.Engine.Compiler.Tracing;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core.Tracing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    /// <summary>
    /// Initialize the code engine framework within the executing application
    /// and ready it for use.
    /// </summary>
    public static class CodeEngineRegistration
    {
        /// <summary>
        /// Use this method to register the code engine within the executing application.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public static void UseCodeEngine(this IServiceCollection services, CodeEngineOptions options)
        {
            services.AddSingleton(options);
            
            if (options.Logger != null)
            {
                services.AddScoped<ILogger>(provider => options.Logger);
            }
            else
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    // Add console logging
                    builder.AddConsole();
                });

                // Create a logger
                services.AddScoped<ILogger>(provider => loggerFactory.CreateLogger("console"));
            }

            if (options.Provider != null)
            {
                services.AddScoped<IActionProvider>(provider => options.Provider);
            }
            
            if (options.Tracer != null)
            {
                services.AddScoped<ITracer>(provider => options.Tracer);
            }
            else
            {
                services.AddScoped<ITracer, Tracer>();
            }
            
            RegisterAllHardcodedExecutors(services);
            RegisterExecutorFactory(services);
        }

        /// <summary>
        /// Register service provider for generic executor injections
        /// </summary>
        /// <param name="services"><c>IServiceCollection</c> for the app builder</param>
        private static void RegisterExecutorFactory(IServiceCollection services)
        {
            services.AddMemoryCache(options =>
            {
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
            });
            
            services.AddScoped<IExecutorFactory, ExecutorFactory>();
            services.AddSingleton<IExecutorCache, ExecutorCache>();
            services.AddScoped(typeof(IExecutor<>), typeof(Executor<>));
            services.AddScoped(typeof(IExecutorCatalog<>), typeof(ExecutorCatalog<>));
        }

        /// <summary>
        /// Register all hardcoded executors in the executing assembly and all referenced assemblies
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterAllHardcodedExecutors(IServiceCollection services)
        {
            var assemblies = GetAllAssemblies().ToList();
            var executorType = typeof(IExecutor<>);
            
            assemblies.ForEach(a =>
            {
                var implementingTypes = a.Value.GetTypes()
                    .Where(t => executorType.IsAssignableFrom(t) && t.IsClass)
                    .ToList();
                
                implementingTypes.ForEach(it =>
                {
                    services.AddScoped(it);
                });
            });
        }

        private static Dictionary<string, Assembly> GetAllAssemblies()
        {
            var allAssemblies = new Dictionary<string, Assembly>();

            var executingAsmb = GetExecutingAndReferencedAssemblies();
            var loadedAsmb = GetLoadedAssemblies();

            executingAsmb.ForEach(a => allAssemblies[a.FullName] = a);
            loadedAsmb.ForEach(a => allAssemblies[a.FullName] = a);

            return allAssemblies;
        }
        
        private static List<Assembly> GetExecutingAndReferencedAssemblies()
        {
            // Get the executing assembly
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            // Get dependent assemblies
            List<Assembly> referencedAssemblies = executingAssembly.GetReferencedAssemblies()
                .Select(Assembly.Load)
                .ToList();
            
            referencedAssemblies.Add(executingAssembly);
            
            return referencedAssemblies;
        }

        private static List<Assembly> GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }
    }
}