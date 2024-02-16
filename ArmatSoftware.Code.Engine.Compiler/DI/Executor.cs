using System;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class Executor<T> : IExecutor<T>
        where T : class, new()
    {

        private readonly IExecutor<T> _executor;
        
        public T Subject
        {
            get => _executor.Subject;
            set => _executor.Subject = value;
        }

        public Executor(ICodeEngineExecutorFactory factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            
            _executor = factory.Provide<T>();
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

        public T Execute(T subject)
        {
            Subject = subject;
            Execute();
            return Subject;
        }

        public IExecutor<T> Clone()
        {
            return (IExecutor<T>) MemberwiseClone();
        }
    }
}