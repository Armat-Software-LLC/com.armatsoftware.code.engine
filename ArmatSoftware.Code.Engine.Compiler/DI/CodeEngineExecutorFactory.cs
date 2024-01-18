using System;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class CodeEngineExecutorFactory : ICodeEngineExecutorFactory
    {
        private CompilerRegistration.CodeEngineRegistrationOptions _options;
        
        public CodeEngineExecutorFactory(CompilerRegistration.CodeEngineRegistrationOptions options)
        {
            _options = options;
        }
        
        public IExecutor<T> Provide<T>()
            where T : class, new()
        {
            var configuration = new CompilerConfiguration<T>(_options.CodeEngineNamespace);

            //todo: replace with actual actions from storage
            configuration.Actions.Add(new CustomAction<T>
            {
                Name = "UpdateData",
                Code = _options.Storage.Retrieve(typeof(T), Guid.Empty)
            });
            
            switch (_options.CompilerType)
            {
                case CompilerTypeEnum.CSharp:
                    var cSharpCompiler = new CSharpCompiler<T>();
                    return cSharpCompiler.Compile(configuration);
                case CompilerTypeEnum.Vb:
                    var vbCompiler = new VbCompiler<T>();
                    return vbCompiler.Compile(configuration);
                default:
                    throw new ArgumentOutOfRangeException(nameof(_options.CompilerType), _options.CompilerType, "Compiler is not found or not implemented");
            };
        }

        public void Dispose()
        {
            //TODO: dispose of the compiler
        }
    }

    public class CustomAction<T> : IAction<T>
        where T : class, new()
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}