using System;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit
{
    [TestFixture]
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
                Options = null;
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
                Options.CodeEngineNamespace = null;
                Target.Provide<CodeEngineFactoryTestSubject>();
            }, Throws.ArgumentNullException);
        }
    }

    public class CodeEngineExecutorFactoryTestBuilder
    {
        protected ICodeEngineExecutorFactory Target { get; private set; }
        
        protected CompilerRegistration.CodeEngineOptions Options { get; set; }
        
        protected ICodeEngineExecutorCache Cache { get; set; }
        
        protected Mock<ICodeEngineStorage> StorageMock { get; private set; }
        protected ICodeEngineStorage Storage { get; set; }
        
        protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
        protected ICodeEngineLogger Logger { get; set; }
        
        [SetUp]
        public void Setup()
        {
            StorageMock = new Mock<ICodeEngineStorage>();
            StorageMock.Setup(x => x.Retrieve(It.Is<Type>(t => t == typeof(CodeEngineFactoryTestSubject)), It.IsAny<Guid>()))
                .Returns("Subject.Data = \"Hello World\";");
            LoggerMock = new Mock<ICodeEngineLogger>();
            
            Storage = StorageMock.Object;
            Logger = LoggerMock.Object;
            Cache = new CodeEngineExecutorCache();
            
            Options = new CompilerRegistration.CodeEngineOptions
            {
                CodeEngineNamespace = "ArmatSoftware.Code.Engine.Tests.Unit",
                CompilerType = CompilerTypeEnum.CSharp,
                Storage = Storage,
                Logger = Logger
            };
        }
        
        public void Build()
        {
            Target = new CodeEngineExecutorFactory(Options, Cache);
        }
    }

    public class CodeEngineFactoryTestSubject
    {
        public string Data { get; set; }
    }

    public class CodeEngineFactoryAction : IAction<CodeEngineFactoryTestSubject>
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
    }
}