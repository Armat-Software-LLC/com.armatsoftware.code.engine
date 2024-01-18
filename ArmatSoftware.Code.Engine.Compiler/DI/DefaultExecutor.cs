using System;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class DefaultExecutor<T> : IExecutor<T>
        where T : class, new()
    {

        private readonly IExecutor<T> _executor;
        
        public T Subject
        {
            get => _executor.Subject;
            set => _executor.Subject = value;
        }

        public DefaultExecutor(ICodeEngineExecutorFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            
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
    }
}