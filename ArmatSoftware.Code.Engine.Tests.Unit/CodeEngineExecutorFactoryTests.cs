using System;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage;
using ArmatSoftware.Code.Engine.Storage.File;
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
        
        protected CodeEngineRegistration.CodeEngineOptions Options { get; set; }
        
        protected ICodeEngineExecutorCache Cache { get; set; }
        
        protected Mock<IActionProvider> StorageMock { get; private set; }
        protected IActionProvider Provider { get; set; }
        
        protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
        protected ICodeEngineLogger Logger { get; set; }

        protected IStoredActions<CodeEngineFactoryTestSubject> Actions { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Actions = new StoredActions<CodeEngineFactoryTestSubject>();
            var action = Actions.Add("SimpleAction");
            action.Update("Subject.Data = \"Hello world!\";", "testauthor", "testcomment");
            action.Activate(1);
            
            StorageMock = new Mock<IActionProvider>();
            StorageMock.Setup(x => x.Retrieve<CodeEngineFactoryTestSubject>())
                .Returns(Actions.ToList());
            LoggerMock = new Mock<ICodeEngineLogger>();
            
            Provider = StorageMock.Object;
            Logger = LoggerMock.Object;
            Cache = new CodeEngineExecutorCache();
            
            Options = new CodeEngineRegistration.CodeEngineOptions
            {
                CodeEngineNamespace = "ArmatSoftware.Code.Engine.Tests.Unit",
                CompilerType = CompilerTypeEnum.CSharp,
                Provider = Provider,
                Logger = Logger
            };
        }
        
        public void Build()
        {
            Target = new CodeEngineExecutorFactory(Options, Provider, Cache);
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