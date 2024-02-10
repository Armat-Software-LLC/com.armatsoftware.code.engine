using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Logger.File;
using ArmatSoftware.Code.Engine.Storage.File;
using Microsoft.Extensions.DependencyInjection;

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
                services.AddScoped<ICodeEngineLogger>(provider => options.Logger);
            }
            else
            {
                services.AddScoped<ICodeEngineLogger, CodeEngineFileLogger>();
            }

            if (options.Provider != null)
            {
                services.AddScoped<IActionProvider>(provider => options.Provider);
            }
            else
            {
                services.AddScoped<IActionProvider, CodeEngineActionProvider>();
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
            services.AddScoped<ICodeEngineExecutorFactory, CodeEngineExecutorFactory>();
            services.AddSingleton<ICodeEngineExecutorCache, CodeEngineExecutorCache>();
            services.AddScoped(typeof(IExecutor<>), typeof(Executor<>));
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

        /// <summary>
        /// Pass to the <c>UseCodeEngine</c> method to configure the code engine.
        /// Make sure to supply at least the <c>CompilerType</c> and <c>CodeEngineNamespace</c> properties.
        /// </summary>
        public class CodeEngineOptions
        {
            /// <summary>
            /// Choose between C# and VB.NET compilers. This depends on the language of the code you use.
            /// </summary>
            public CompilerTypeEnum CompilerType { get; set; }
            
            /// <summary>
            /// Namespace where all generated executors will be placed.
            /// Make sure it is different from the namespace of the code you use.
            /// </summary>
            public string CodeEngineNamespace { get; set; }

            /// <summary>
            /// Optionally, provide a logger to use for the code engine.
            /// If none is provided, a <c>CodeEngineFileLogger</c> will be used.
            /// </summary>
            public ICodeEngineLogger Logger { get; set; }
            
            /// <summary>
            /// Optionally, provide a storage to use for the code engine.
            /// If none provided, a <c>CodeEngineFileStorage</c> will be used.
            /// </summary>
            public IActionProvider Provider { get; set; }
        }
    }
}