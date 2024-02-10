using System;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Storage;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class CodeEngineExecutorFactory : ICodeEngineExecutorFactory
    {
        private readonly CodeEngineRegistration.CodeEngineOptions _options;
        
        private readonly ICodeEngineExecutorCache _cache;
        private readonly IActionProvider _actionProvider;

        public CodeEngineExecutorFactory(
            CodeEngineRegistration.CodeEngineOptions options,
            IActionProvider actionProvider,
            ICodeEngineExecutorCache cache)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _actionProvider = actionProvider ?? throw new ArgumentNullException(nameof(actionProvider));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        
        public IExecutor<T> Provide<T>()
            where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(_options.CodeEngineNamespace))
            {
                throw new ArgumentNullException(nameof(_options.CodeEngineNamespace));
            }
            
            var configuration = new CompilerConfiguration<T>(_options.CodeEngineNamespace);

            // check cache and return new instance if found
            var cachedExecutor = _cache.Retrieve<T>();
            if (cachedExecutor != null)
            {
                return cachedExecutor.Clone();
            }
            
            configuration.Actions = _actionProvider.Retrieve<T>().ToList();
            
            // compile new executors and cache them before returning
            switch (_options.CompilerType)
            {
                case CompilerTypeEnum.CSharp:
                    var cSharpCompiler = new CSharpCompiler<T>();
                    var cSharpExecutor = cSharpCompiler.Compile(configuration);
                    _cache.Cache(cSharpExecutor);
                    return cSharpExecutor.Clone();
                case CompilerTypeEnum.Vb:
                    var vbCompiler = new VbCompiler<T>();
                    var vbExecutor = vbCompiler.Compile(configuration);
                    _cache.Cache(vbExecutor);
                    return vbExecutor.Clone();
                default:
                    throw new ArgumentOutOfRangeException(nameof(_options.CompilerType), _options.CompilerType, "Compiler is not found or not implemented");
            }
        }

        public void Dispose()
        {
            _cache.Clear();
        }
    }
    
    public class SubjectAction<T> : ISubjectAction<T>
        where T : class, new()
    {
        public string Name { get; set; }
        public string Code { get; set; }
        
        public int Order { get; set; }
    }
}

/// <summary>
/// IExecutor factory is used to create new instances of IExecutor for the supplied
/// subject type. Caching is used for performance.
/// </summary>
public interface ICodeEngineExecutorFactory : IDisposable
{
    IExecutor<T> Provide<T>()
        where T : class, new();
}