using System;
using System.Collections.Generic;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class CodeEngineExecutorCache : ICodeEngineExecutorCache
    {
        private readonly IDictionary<Type, object> _cache = new Dictionary<Type, object>();
        
        public void Cache<T>(IExecutor<T> executor)
            where T : class, new()
        {
            if (executor == null)
            {
                throw new ArgumentNullException(nameof(executor));
            }
            
            _cache[typeof(T)] = executor;
        }

        public IExecutor<T> Retrieve<T>()
            where T : class, new()
        {
            if (_cache.TryGetValue(typeof(T), out var executor))
            {
                return (IExecutor<T>) executor;
            }

            return null;
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }

    public interface ICodeEngineExecutorCache
    {
        public void Cache<T>(IExecutor<T> executor)
            where T : class, new();
        
        public IExecutor<T> Retrieve<T>()
            where T : class, new();
        
        public void Clear();
    }
}