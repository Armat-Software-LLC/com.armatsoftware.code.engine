// Use to toggle cacheability of the factory
// #define NOCACHE

using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core.Tracing;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.Execution
{
    public class ExecutorFactory(
        CodeEngineOptions options,
        IActionProvider actionProvider,
        IExecutorCache cache,
        ITracer tracer,
        ILogger logger)
        : IExecutorFactory
    {
        private readonly CodeEngineOptions _options = options ?? throw new ArgumentNullException(nameof(options));
        private readonly IExecutorCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(options.Logger));
        private readonly IActionProvider _actionProvider = actionProvider ?? throw new ArgumentNullException(nameof(actionProvider));
        private readonly ITracer _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        
        /// <summary>
        /// Default factory implementation for the <c>Provide()</c> method.
        /// Uses caching and registered <c>IActionProvider</c> to retrieve executor instances
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TSubject"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IExecutor<TSubject> Provide<TSubject>(string key = "")
            where TSubject : class, new()
        {
            if (string.IsNullOrWhiteSpace(_options.CodeEngineNamespace))
            {
                throw new ArgumentNullException(nameof(_options.CodeEngineNamespace));
            }
            
            var configuration = new CompilerConfiguration<TSubject>(_options.CodeEngineNamespace);

#if !NOCACHE
            // check cache and return new instance if found
            var cachedExecutor = _cache.Retrieve<TSubject>(key);
            if (cachedExecutor != null)
            {
                return ManufactureClone(cachedExecutor, key);
            }
#endif
            
            configuration.Actions = _actionProvider.Retrieve<TSubject>(key).ToList();

            configuration.ValidateModelsAfterExecution = _options.ValidateModelsAfterExecution;
            
            // compile new executors and cache them before returning

            IFactoryExecutor<TSubject> compiledExecutor;
            switch (_options.CompilerType)
            {
                case CompilerTypeEnum.CSharp:
                    var cSharpCompiler = new CSharpCompiler<TSubject>();
                    compiledExecutor = cSharpCompiler.Compile(configuration);
                    break;
                case CompilerTypeEnum.Vb:
                    var vbCompiler = new VbCompiler<TSubject>();
                    compiledExecutor = vbCompiler.Compile(configuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_options.CompilerType), _options.CompilerType, "Compiler is not found or not implemented");
            }
#if !NOCACHE
            _cache.Cache(compiledExecutor, key);
#endif
            return ManufactureClone(compiledExecutor, key);
        }
        
        /// <summary>
        /// Try to configure a new instance of Factory executor
        /// before cloning it
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="key"></param>
        /// <typeparam name="TSubject"></typeparam>
        /// <returns></returns>
        private IExecutor<TSubject> ManufactureClone<TSubject>(IFactoryExecutor<TSubject> executor, string key)
            where TSubject : class, new()
        {
            _logger.BeginScope(new LogContext
            {
                Key = key,
                SubjectType = typeof(TSubject).FullName,
                CorrelationId = _tracer.CorrelationId
            });
            
            executor.SetLogger(_logger);
            
            return executor.Clone();
        }
    }
}

/// <summary>
/// IExecutor factory is used to create new instances of IExecutor for the supplied
/// subject type. Caching is used for performance.
/// </summary>
public interface IExecutorFactory
{
    /// <summary>
    /// Provide a new instance of IExecutor for the supplied subject type
    /// ans key, if one is provided
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IExecutor<TSubject> Provide<TSubject>(string key = "")
        where TSubject : class, new();
}