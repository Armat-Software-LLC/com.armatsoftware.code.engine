using System;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class Executor<TSubject> : IExecutor<TSubject>
        where TSubject : class, new()
    {

        private readonly IExecutor<TSubject> _executor;

        public TSubject Subject => _executor.Subject;

        public Executor(ICodeEngineExecutorFactory factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _executor = factory.Provide<TSubject>();
        }

        public void Save(string key, object value)
        {
            _executor.Save(key, value);
        }

        public object Read(string key)
        {
            return _executor.Read(key);
        }

        public TSubject Execute(TSubject subject)
        {
            return _executor.Execute(subject);
        }

        public ICodeEngineLogger Log => _executor.Log;

        public IExecutor<TSubject> Clone() => _executor.Clone();
        
    }
}