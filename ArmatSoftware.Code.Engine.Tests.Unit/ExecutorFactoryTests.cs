using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Compiler.Execution;
using ArmatSoftware.Code.Engine.Compiler.Tracing;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core.Tracing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
    [TestFixture, TestOf(typeof(ExecutorFactory))]
    public class ExecutorFactoryTests : CodeEngineExecutorFactoryTestBuilder
    {
        [Test]
        public void Should_Construct()
        {
            Build();
            Assert.That(Target, Is.Not.Null);
        }
        
        [Test]
        public void Should_Not_Construct_With_Cache_Null()
        {
            Assert.That(() =>
            {
                Cache = null;
                Build();
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Not_Construct_With_Options_Null()
        {
            Assert.That(() =>
            {
                RegistrationOptions = null;
                Build();
            }, Throws.ArgumentNullException);
        }

        [Test]
        public void Should_Construct_With_Logger_Null()
        {
            Logger = null;
            Build();
            Assert.That(Target, Is.Not.Null);
        }

        [Test]
        public void Should_Construct_With_Storage_Null()
        {
            Logger = null;
            Build();
            Assert.That(Target, Is.Not.Null);
        }
        
        [Test]
        public void Should_Provide_With_Valid_Subject_Type()
        {
            Build();
            var result = Target.Provide<CodeEngineFactoryTestSubject>();
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public void Should_Not_Provide_With_Null_Namespace()
        {
            Assert.That(() =>
            {
                Build();
                RegistrationOptions.CodeEngineNamespace = null;
                Target.Provide<CodeEngineFactoryTestSubject>();
            }, Throws.ArgumentNullException);
        }
    }

    public class CodeEngineExecutorFactoryTestBuilder
    {
        protected IExecutorFactory Target { get; private set; }
        
        protected CodeEngineOptions RegistrationOptions { get; set; }
        
        protected IExecutorCache Cache { get; set; }
        
        protected ITracer Tracer { get; set; }
        
        protected Mock<IActionProvider> StorageMock { get; private set; }
        protected IActionProvider Provider { get; set; }
        
        protected Mock<ILogger> LoggerMock { get; private set; }
        protected ILogger Logger { get; set; }
        
        protected IMemoryCache MemCache { get; set; }

        protected List<TestSubjectAction<CodeEngineFactoryTestSubject>> SubjectActions { get; set; }
        
        [SetUp]
        public void Setup()
        {
            SubjectActions = new List<TestSubjectAction<CodeEngineFactoryTestSubject>>()
            {
                new TestSubjectAction<CodeEngineFactoryTestSubject>()
                {
                    Name = "SimpleAction",
                    Code = "Subject.Data = \"Hello world!\";",
                    Order = 1
                }
            };
            
            StorageMock = new Mock<IActionProvider>();
            StorageMock.Setup(x => x.Retrieve<CodeEngineFactoryTestSubject>(It.IsAny<string>()))
                .Returns(SubjectActions.ToList());
            LoggerMock = new Mock<ILogger>();
            
            Provider = StorageMock.Object;
            Logger = LoggerMock.Object;
            
            RegistrationOptions = new CodeEngineOptions
            {
                CodeEngineNamespace = "ArmatSoftware.Code.Engine.Tests.Unit",
                CompilerType = CompilerTypeEnum.CSharp,
                Provider = Provider,
                Logger = Logger
            };
            
            MemCache = new MemoryCache(Options.Create(new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(1)
            }));
            
            Cache = new ExecutorCache(MemCache, RegistrationOptions);

            Tracer = new Tracer();
        }
        
        public void Build()
        {
            Target = new ExecutorFactory(RegistrationOptions, Provider, Cache, Tracer);
        }
    }

    public class CodeEngineFactoryTestSubject
    {
        public string Data { get; set; }
    }
}