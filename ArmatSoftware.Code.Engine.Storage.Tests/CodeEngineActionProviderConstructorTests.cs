using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Logging;
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
    protected Mock<ILogger> LoggerMock { get; private set; }
    protected ILogger Logger { get; set; }
    
    protected Mock<IStorageAdapter> StorageAdapterMock { get; private set; }
    protected IStorageAdapter StorageAdapter { get; set; }
        
    [SetUp]
    public void Setup()
    {
        LoggerMock = new Mock<ILogger>();
        StorageAdapterMock = new Mock<IStorageAdapter>();
        
        Logger = LoggerMock.Object;
        StorageAdapter = StorageAdapterMock.Object;
    }

    protected IActionProvider Build()
    {
        return new CodeEngineActionProvider(Logger, StorageAdapter);
    }
}