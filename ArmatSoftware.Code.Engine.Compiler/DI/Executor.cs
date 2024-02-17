using System;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class Executor<TSubject> : IExecutor<TSubject>
        where TSubject : class, new()
    {

        private readonly IFactoryExecutor<TSubject> _factoryExecutor;

        public TSubject Subject => _factoryExecutor.Subject;

        public Executor(ICodeEngineExecutorFactory factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _factoryExecutor = factory.Provide<TSubject>();
        }
        
        public void Save(string key, object value)
        {
            _factoryExecutor.Save(key, value);
        }

        public object Read(string key)
        {
            return _factoryExecutor.Read(key);
        }

        public TSubject Execute(TSubject subject)
        {
            _factoryExecutor.Execute(subject);
            return _factoryExecutor.Subject;
        }

        public ICodeEngineLogger Log => _factoryExecutor.Log;
    }
}