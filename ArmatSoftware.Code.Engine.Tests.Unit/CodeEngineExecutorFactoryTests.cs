using System;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage.File;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
    [TestFixture, TestOf(typeof(CodeEngineExecutorFactory))]
    public class CodeEngineExecutorFactoryTests : CodeEngineExecutorFactoryTestBuilder
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
        protected ICodeEngineExecutorFactory Target { get; private set; }
        
        protected CodeEngineOptions RegistrationOptions { get; set; }
        
        protected ICodeEngineExecutorCache Cache { get; set; }
        
        protected Mock<IActionProvider> StorageMock { get; private set; }
        protected IActionProvider Provider { get; set; }
        
        protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
        protected ICodeEngineLogger Logger { get; set; }
        
        // protected Mock<IMemoryCache> MemoryCacheMock { get; private set; }
        
        protected IMemoryCache MemCache { get; set; }

        protected StoredActions<CodeEngineFactoryTestSubject> Actions { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Actions = new StoredActions<CodeEngineFactoryTestSubject>();
            var action = Actions.Add("SimpleAction");
            action.Update("Subject.Data = \"Hello world!\";", "testauthor", "testcomment");
            action.Activate(1);
            
            StorageMock = new Mock<IActionProvider>();
            StorageMock.Setup(x => x.Retrieve<CodeEngineFactoryTestSubject>(It.IsAny<string>()))
                .Returns(Actions.ToList());
            LoggerMock = new Mock<ICodeEngineLogger>();
            // MemoryCacheMock = new Mock<IMemoryCache>();
            
            Provider = StorageMock.Object;
            Logger = LoggerMock.Object;
            // MemoryCache = MemoryCacheMock.Object;
            
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
            
            Cache = new CodeEngineExecutorCache(MemCache, RegistrationOptions);
        }
        
        public void Build()
        {
            Target = new CodeEngineExecutorFactory(RegistrationOptions, Provider, Cache);
        }
    }

    public class CodeEngineFactoryTestSubject
    {
        public string Data { get; set; }
    }

    public class CodeEngineFactorySubjectAction : ISubjectAction<CodeEngineFactoryTestSubject>
    {
        public string Name
        {
            get => "UpdateData";
            set => throw new NotImplementedException();
        }
        
        public string Code
        {
            get => "Subject.Data = \"Hello World\";";
            set => throw new NotImplementedException();
        }

        public int Order
        {
            get => 0;
            set => throw new NotImplementedException();
        }
    }
}