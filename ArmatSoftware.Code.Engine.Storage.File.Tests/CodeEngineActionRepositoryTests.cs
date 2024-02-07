using System;
using System.IO;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests;

public class CodeEngineActionRepositoryTests : CodeEngineActionRepositoryTestBuilder
{
    [TestFixture]
    public class CodeEngineActionRepositoryConstructorTests : CodeEngineActionRepositoryTestBuilder
    {
        [Test]
        public void Should_Construct_With_Valid_Parameters()
        {
            Assert.That(() => 
            {
                Build(); 
            }, Throws.Nothing);
        }
        
        [Test]
        public void Should_Fail_With_Null_Configuration()
        {
            Assert.That(() =>
            {
                new CodeEngineActionRepository(null, Logger);
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_With_Null_Logger()
        {
            Assert.That(() =>
            {
                new CodeEngineActionRepository(Configuration, null);
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_With_Null_Parameters()
        {
            Assert.That(() =>
            {
                new CodeEngineActionRepository(null, null);
            }, Throws.ArgumentNullException);
        }
    }
    
    [TestFixture]
    public class CodeEngineActionRepositoryStoreTests : CodeEngineActionRepositoryTestBuilder
    {
    }

    [TestFixture]
    public class CodeEngineActionRepositoryReadTests : CodeEngineActionRepositoryTestBuilder
    {
    }
    
    [TestFixture]
    public class CodeEngineActionRepositoryAddTests : CodeEngineActionRepositoryTestBuilder
    {
    }
    
    [TestFixture]
    public class CodeEngineActionRepositoryReorderTests : CodeEngineActionRepositoryTestBuilder
    {
    }
    
    [TestFixture]
    public class CodeEngineActionRepositoryRemoveTests : CodeEngineActionRepositoryTestBuilder
    {
    }

    // using for test only
    public class CustomTestType
    { }
}

public abstract class CodeEngineActionRepositoryTestBuilder
{
    protected Mock<IConfigurationRoot> ConfigurationMock { get; private set; }
    protected IConfigurationRoot Configuration { get; private set; }
    protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
    protected ICodeEngineLogger Logger { get; private set; }
    
    [SetUp]
    public void Setup()
    {
        ConfigurationMock = new Mock<IConfigurationRoot>();
        LoggerMock = new Mock<ICodeEngineLogger>();
    }

    protected IActionRepository Build()
    {
        Configuration = ConfigurationMock.Object;

        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStoragePath)])
            .Returns(Path.GetTempPath());
    
        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStorageExtension)])
            .Returns("test");

        Logger = LoggerMock.Object;

        return new CodeEngineActionRepository(Configuration, Logger);
    }
}