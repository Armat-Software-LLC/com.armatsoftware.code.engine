using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Core;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
    [TestFixture]
    public class CodeEngineExecutorCacheTests : CodeEngineExecutorCacheTestBuilder
    {
        [Test]
        public void Should_Cache_And_Retrieve_By_SubjectType()
        {
            Build();

            Target.Cache(Executor);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));
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
        public void Should_Cache_And_Clear_By_SubjectType()
        {
            Build();

            Target.Cache(Executor);

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.EqualTo(Executor));
            
            Target.Clear();

            Assert.That(Target.Retrieve<TestExecutorCacheSubject>(), Is.Null);
        }
    }
    
    public class CodeEngineExecutorCacheTestBuilder
    {
        protected ICodeEngineExecutorCache Target { get; set; }
            
        private Mock<IExecutor<TestExecutorCacheSubject>> ExecutorMock { get; set; }
        protected IExecutor<TestExecutorCacheSubject> Executor { get; set; }

        [SetUp]
        public void Init()
        {
            ExecutorMock = new Mock<IExecutor<TestExecutorCacheSubject>>();
        }
            
        public void Build()
        {
            Executor = ExecutorMock.Object;
            Target = new CodeEngineExecutorCache();
        }
    }

    public class TestExecutorCacheSubject
    {
        public string Data { get; set; }
    }
}