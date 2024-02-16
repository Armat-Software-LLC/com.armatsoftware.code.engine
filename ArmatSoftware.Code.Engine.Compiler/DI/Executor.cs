using System;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class Executor<TSubject> : IExecutor<TSubject>
        where TSubject : class, new()
    {

        private readonly IExecutor<TSubject> _compiledExecutor;

        public TSubject Subject
        {
            get => _compiledExecutor.Subject;
            set => _compiledExecutor.Subject = value;
        }

        public Executor(ICodeEngineExecutorFactory factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _compiledExecutor = factory.Provide<TSubject>();
        }
        
        public void Save(string key, object value)
        {
            _compiledExecutor.Save(key, value);
        }

        public object Read(string key)
        {
            return _compiledExecutor.Read(key);
        }
        
        public void Execute()
        {
            _compiledExecutor.Execute();
        }

        public TSubject Execute(TSubject subject)
        {
            _compiledExecutor.Subject = subject;
            Execute();
            return Subject;
        }

        public IExecutor<TSubject> Clone()
        {
            return (IExecutor<TSubject>) MemberwiseClone();
        }

        public void Info(string message)
        {
            throw new NotImplementedException();
        }
    }
}