// Use to toggle cacheability of the factory
// #define NOCACHE

using System;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class CodeEngineExecutorFactory(
        CodeEngineOptions options,
        IActionProvider actionProvider,
        ICodeEngineExecutorCache cache)
        : ICodeEngineExecutorFactory
    {
        private readonly CodeEngineOptions _options = options ?? throw new ArgumentNullException(nameof(options));
        
        private readonly ICodeEngineExecutorCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ICodeEngineLogger _logger = options.Logger ?? throw new ArgumentNullException(nameof(options.Logger));
        private readonly IActionProvider _actionProvider = actionProvider ?? throw new ArgumentNullException(nameof(actionProvider));

        /// <summary>
        /// Default factory implementation for the <c>Provide()</c> method.
        /// Uses caching and registered <c>IActionProvider</c> to retrieve executor instances
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IExecutor<T> Provide<T>(string key = "")
            where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(_options.CodeEngineNamespace))
            {
                throw new ArgumentNullException(nameof(_options.CodeEngineNamespace));
            }
            
            var configuration = new CompilerConfiguration<T>(_options.CodeEngineNamespace);

#if !NOCACHE
            // check cache and return new instance if found
            var cachedExecutor = _cache.Retrieve<T>(key);
            if (cachedExecutor != null)
            {
                return CloneAndInitialize(cachedExecutor);
            }
#endif
            
            configuration.Actions = _actionProvider.Retrieve<T>(key).ToList();
            
            // compile new executors and cache them before returning

            IExecutor<T> compiledExecutor;
            switch (_options.CompilerType)
            {
                case CompilerTypeEnum.CSharp:
                    var cSharpCompiler = new CSharpCompiler<T>();
                    compiledExecutor = cSharpCompiler.Compile(configuration);
                    break;
                case CompilerTypeEnum.Vb:
                    var vbCompiler = new VbCompiler<T>();
                    compiledExecutor = vbCompiler.Compile(configuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_options.CompilerType), _options.CompilerType, "Compiler is not found or not implemented");
            }
#if !NOCACHE
            _cache.Cache(compiledExecutor, key);
#endif
            return CloneAndInitialize(compiledExecutor);
        }
        
        private IExecutor<TSubject> CloneAndInitialize<TSubject>(IExecutor<TSubject> executor)
            where TSubject : class, new()
        {
            var clonedExecutor = executor.Clone();
            // TODO: clonedExecutor.Log = _logger;
            return clonedExecutor;
        }
    }
}

/// <summary>
/// IExecutor factory is used to create new instances of IExecutor for the supplied
/// subject type. Caching is used for performance.
/// </summary>
public interface ICodeEngineExecutorFactory
{
    /// <summary>
    /// Provide a new instance of IExecutor for the supplied subject type
    /// ans key, if one is provided
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IExecutor<T> Provide<T>(string key = "")
        where T : class, new();
}