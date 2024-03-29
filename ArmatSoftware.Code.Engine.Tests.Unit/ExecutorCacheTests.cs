using System;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Compiler.Execution;
using ArmatSoftware.Code.Engine.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
    [TestFixture, TestOf(typeof(ExecutorCache))]
    public class ExecutorCacheTests : CodeEngineExecutorCacheTestBuilder
    {
        private const string TestKey = "test_key";
        
        [Test]
        public void Should_Cache_And_Retrieve_By_SubjectType()
        {
            Build();

            Target.Cache(Executor);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));
        }
        
        [Test]
        public void Should_Cache_And_Retrieve_By_SubjectType_And_Key()
        {
            Build();

            Target.Cache(Executor, TestKey);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(TestKey), Is.EqualTo(Executor));
        }
        
        [Test]
        public void Should_Cache_And_Overwrite_By_SubjectType()
        {
            Build();
            
            Target.Cache(Executor);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));

            Target.Cache(Executor);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));
        }
        
        [Test]
        public void Should_Cache_And_Overwrite_By_SubjectType_And_Key()
        {
            Build();
            
            Target.Cache(Executor, TestKey);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(TestKey), Is.EqualTo(Executor));

            Target.Cache(Executor, TestKey);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(TestKey), Is.EqualTo(Executor));
        }
        
        [Test]
        public void Should_Cache_And_Clear_By_SubjectType()
        {
            Build();

            Target.Cache(Executor);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));
            
            Target.Clear<TestExecutorCacheSubject>();

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.Null);
        }
        
        [Test]
        public void Should_Cache_And_Clear_By_SubjectType_And_Key()
        {
            Build();

            Target.Cache(Executor, TestKey);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(TestKey), Is.EqualTo(Executor));
            
            Target.Clear<TestExecutorCacheSubject>(TestKey);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(TestKey), Is.Null);
        }

        [Test]
        public void Should_Not_Clear_SubjectType_Cache_By_Key()
        {
            Build();

            Target.Cache(Executor);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));
            
            Target.Clear<TestExecutorCacheSubject>(TestKey);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));
        }

        [Test]
        public void Should_Not_Clear_Keyed_Cache_By_SubjectType()
        {
            Build();

            Target.Cache(Executor, TestKey);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(TestKey), Is.EqualTo(Executor));
            
            Target.Clear<TestExecutorCacheSubject>();

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(TestKey), Is.EqualTo(Executor));
        }
    }
    
    public class CodeEngineExecutorCacheTestBuilder
    {
        protected IExecutorCache Target { get; set; }
            
        private Mock<IFactoryExecutor<TestExecutorCacheSubject>> ExecutorMock { get; set; }
        protected IFactoryExecutor<TestExecutorCacheSubject> Executor { get; set; }
        
        protected IMemoryCache MemCache { get; set; }

        [SetUp]
        public void Init()
        {
            ExecutorMock = new Mock<IFactoryExecutor<TestExecutorCacheSubject>>();
        }
            
        protected void Build()
        {
            MemCache = new MemoryCache(Options.Create(new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(1)
            }));
            Executor = ExecutorMock.Object;
            Target = new ExecutorCache(MemCache, new CodeEngineOptions()
            {
                CacheExpirationMinutes = 1
            });
        }
    }

    public class TestExecutorCacheSubject
    {
        public string Data { get; set; }
    }
}