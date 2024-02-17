using System;
using ArmatSoftware.Code.Engine.Core;
using Microsoft.Extensions.Caching.Memory;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public class CodeEngineExecutorCache(IMemoryCache cacheContainer, CodeEngineOptions options)
        : ICodeEngineExecutorCache
    {
        private const string CacheKeyPrefix = "CodeEngineExecutorCache";

        private readonly IMemoryCache _cache = cacheContainer ?? throw new ArgumentNullException(nameof(cacheContainer));
        private readonly double _expiration = options.CacheExpirationMinutes;

        // retrieve the cache state from the previous instance into this singleton

        private string GetCacheKey<T>(string key)
            where T : class
        {
            return $"{CacheKeyPrefix}:{typeof(T).FullName}:{key}";
        }
        
        public void Cache<T>(IFactoryExecutor<T> executor, string key = "")
            where T : class, new()
        {
            var cacheKey = GetCacheKey<T>(key);

            _cache.Set(cacheKey, executor, TimeSpan.FromMinutes(_expiration));
        }

        public IFactoryExecutor<T> Retrieve<T>(string key = "")
            where T : class, new()
        {
            var cacheKey = GetCacheKey<T>(key);
            
            return _cache.TryGetValue<IFactoryExecutor<T>>(cacheKey, out var executor) ? executor : null;
        }

        public void Clear<T>(string key = "") where T : class, new()
        {
           _cache.Remove(GetCacheKey<T>(key));
        }
    }

    public interface ICodeEngineExecutorCache
    {
        void Cache<T>(IFactoryExecutor<T> executor, string key = "")
            where T : class, new();
        
        IFactoryExecutor<T> Retrieve<T>(string key = "")
            where T : class, new();
        
        void Clear<T>(string key = "")
            where T : class, new();
    }
}