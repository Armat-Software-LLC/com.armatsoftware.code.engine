using System.IO;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.Tests;

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
            Configuration = null;
            Build();
        }, Throws.ArgumentNullException);
    }
    
    [Test]
    public void Should_Fail_With_Null_Logger()
    {
        Assert.That(() =>
        {
            Logger = null;
            Build();
        }, Throws.ArgumentNullException);
    }
    
    [Test]
    public void Should_Fail_With_Null_Storage()
    {
        Assert.That(() =>
        {
            StorageAdapter = null;
            Build();
        }, Throws.ArgumentNullException);
    }
}

public abstract class CodeEngineActionProviderTestsBuilder
{
    protected Mock<IConfigurationRoot> ConfigurationMock { get; private set; }
    protected IConfigurationRoot Configuration { get; set; }
    
    protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
    protected ICodeEngineLogger Logger { get; set; }
    
    protected Mock<IStorageAdapter> StorageAdapterMock { get; private set; }
    protected IStorageAdapter StorageAdapter { get; set; }
        
    [SetUp]
    public void Setup()
    {
        ConfigurationMock = new Mock<IConfigurationRoot>();
        LoggerMock = new Mock<ICodeEngineLogger>();
        StorageAdapterMock = new Mock<IStorageAdapter>();
        
        Configuration = ConfigurationMock.Object;

        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStoragePath)])
            .Returns(Path.GetTempPath());
        
        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStorageExtension)])
            .Returns("log");

        Logger = LoggerMock.Object;
        
        StorageAdapter = StorageAdapterMock.Object;
    }

    protected IActionProvider Build()
    {
        return new CodeEngineActionProvider(Configuration, Logger, StorageAdapter);
    }
}