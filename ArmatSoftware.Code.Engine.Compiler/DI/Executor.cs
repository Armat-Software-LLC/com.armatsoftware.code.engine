using System;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class Executor<TSubject> : IExecutor<TSubject>
        where TSubject : class, new()
    {

        private readonly IExecutor<TSubject> _executor;
        private readonly ICodeEngineLogger _logger;

        public TSubject Subject
        {
            get => _executor.Subject;
            set => _executor.Subject = value;
        }

        public Executor(ICodeEngineExecutorFactory factory, ICodeEngineLogger logger)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        
        public void Execute()
        {
            _executor.Execute();
        }

        public TSubject Execute(TSubject subject)
        {
            Subject = subject;
            Execute();
            return Subject;
        }

        public IExecutor<TSubject> Clone()
        {
            return (IExecutor<TSubject>) MemberwiseClone();
        }

        public ICodeEngineLogger Log
        {
            get => _executor.Log;
            set => _executor.Log = value;
        }
    }
}