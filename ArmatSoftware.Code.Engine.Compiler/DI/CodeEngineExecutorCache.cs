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

        private string GetCacheKey<TSubject>(string key)
            where TSubject : class
        {
            return $"{CacheKeyPrefix}:{typeof(TSubject).FullName}:{key}";
        }
        
        public void Cache<TSubject>(IFactoryExecutor<TSubject> executor, string key = "")
            where TSubject : class, new()
        {
            var cacheKey = GetCacheKey<TSubject>(key);

            _cache.Set(cacheKey, executor, TimeSpan.FromMinutes(_expiration));
        }

        public IFactoryExecutor<TSubject> Retrieve<TSubject>(string key = "")
            where TSubject : class, new()
        {
            var cacheKey = GetCacheKey<TSubject>(key);
            
            return _cache.TryGetValue<IFactoryExecutor<TSubject>>(cacheKey, out var executor) ? executor : null;
        }

        public void Clear<TSubject>(string key = "") where TSubject : class, new()
        {
           _cache.Remove(GetCacheKey<TSubject>(key));
        }
    }

    public interface ICodeEngineExecutorCache
    {
        void Cache<TSubject>(IFactoryExecutor<TSubject> executor, string key = "")
            where TSubject : class, new();
        
        IFactoryExecutor<TSubject> Retrieve<TSubject>(string key = "")
            where TSubject : class, new();
        
        void Clear<TSubject>(string key = "")
            where TSubject : class, new();
    }
}