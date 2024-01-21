using System;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class CodeEngineExecutorFactory : ICodeEngineExecutorFactory
    {
        private readonly CompilerRegistration.CodeEngineOptions _options;
        
        private readonly ICodeEngineExecutorCache _cache;
            
        public CodeEngineExecutorFactory(CompilerRegistration.CodeEngineOptions options, ICodeEngineExecutorCache cache)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        
        public IExecutor<T> Provide<T>()
            where T : class, new()
        {
            var configuration = new CompilerConfiguration<T>(_options.CodeEngineNamespace);

            // check cache and return new instance if found
            var cachedExecutor = _cache.Retrieve<T>();
            if (cachedExecutor != null)
            {
                return cachedExecutor.Clone();
            }
            
            // add the default actions
            configuration.Actions.Add(new Action<T>
            {
                Name = "UpdateData",
                Code = _options.Storage.Retrieve(typeof(T), Guid.Empty)
            });
            
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
    
    public class Action<T> : IAction<T>
        where T : class, new()
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}

/// <summary>
/// 
/// </summary>
public interface ICodeEngineExecutorFactory : IDisposable
{
    IExecutor<T> Provide<T>()
        where T : class, new();
}