using System.IO;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests;

[TestFixture]
public class CodeEngineActionProviderConstructorTests : CodeEngineActionProviderTestsBuilder
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
            new CodeEngineActionProvider(null, Logger);
        }, Throws.ArgumentNullException);
    }
    
    [Test]
    public void Should_Fail_With_Null_Logger()
    {
        Assert.That(() =>
        {
            new CodeEngineActionProvider(Configuration, null);
        }, Throws.ArgumentNullException);
    }
    
    [Test]
    public void Should_Fail_With_Null_Parameters()
    {
        Assert.That(() =>
        {
            new CodeEngineActionProvider(null, null);
        }, Throws.ArgumentNullException);
    }
}

public abstract class CodeEngineActionProviderTestsBuilder
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

    protected IActionProvider Build()
    {
        Configuration = ConfigurationMock.Object;

        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStoragePath)])
            .Returns(Path.GetTempPath());
        
        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStorageExtension)])
            .Returns("log");

        Logger = LoggerMock.Object;

        return new CodeEngineActionProvider(Configuration, Logger);
    }
}