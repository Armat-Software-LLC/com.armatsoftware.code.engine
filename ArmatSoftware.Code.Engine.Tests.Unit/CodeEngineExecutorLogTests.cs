using System;
using System.Collections.Generic;
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

namespace ArmatSoftware.Code.Engine.Tests.Unit;

[TestFixture]
public class CodeEngineExecutorLogTests : CodeEngineExecutorLogTestBuilder
{
    [Test]
    public void Should_Log_Info_Message()
    {
        var executor = Build();

        executor.Execute(new TestSubject());
        
        Assert.That(Logger.InfoLog.Any(x => x == InfoMessage));
    }
    
    [Test]
    public void Should_Log_Error_Message()
    {
        var executor = Build();
        
        executor.Execute(new TestSubject());
        
        Assert.That(Logger.ErrorLog.Any(x => x == ErrorMessage));
    }
    
    [Test]
    public void Should_Log_Warning_Message()
    {
        var executor = Build();
        
        executor.Execute(new TestSubject());
        
        Assert.That(Logger.WarningLog.Any(x => x == WarningMessage));
    }
}


public class CodeEngineExecutorLogTestBuilder
{
    protected const string InfoMessage = "info message";
    protected const string ErrorMessage = "error message";
    protected const string WarningMessage = "warning message";
    
    protected ICodeEngineExecutorFactory Target { get; private set; }
    
    protected CodeEngineOptions RegistrationOptions { get; set; }
    
    protected ICodeEngineExecutorCache Cache { get; set; }
    
    protected Mock<IActionProvider> StorageMock { get; private set; }
    protected IActionProvider Provider { get; set; }
    
    protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
    protected CodeEngineExecutorLogTestLogger Logger { get; set; }
    
    protected IMemoryCache MemCache { get; set; }

    protected StoredActions<TestSubject> Actions { get; set; }
    
    protected CodeEngineExecutorFactory Factory { get; set; }

    [SetUp]
    public void Setup()
    {
        Actions = new StoredActions<TestSubject>();
        
        var logInfoAction = Actions.Add("LogInfo");
        logInfoAction.Update($"Log.Info(\"{InfoMessage}\"); Log = null;", "testauthor", "testcomment");
        logInfoAction.Activate(1);
        
        var logWarningAction = Actions.Add("LogWarning");
        logWarningAction.Update($"Log.Warning(\"{WarningMessage}\");", "testauthor", "testcomment");
        logWarningAction.Activate(1);
        
        var logErrorAction = Actions.Add("LogError");
        logErrorAction.Update($"Log.Error(\"{ErrorMessage}\");", "testauthor", "testcomment");
        logErrorAction.Activate(1);
        
        StorageMock = new Mock<IActionProvider>();
        StorageMock.Setup(x => x.Retrieve<TestSubject>(It.IsAny<string>()))
            .Returns(Actions.ToList());

        Provider = StorageMock.Object;
        Logger = new CodeEngineExecutorLogTestLogger();
        
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
        
        Factory = new CodeEngineExecutorFactory(RegistrationOptions, Provider, Cache);
    }
    
    protected IExecutor<TestSubject> Build()
    {
        return Factory.Provide<TestSubject>();
    }
}

public class CodeEngineExecutorLogTestLogger : ICodeEngineLogger
{
    public List<string> InfoLog { get; } = new();
    public List<string> ErrorLog { get; } = new();
    public List<string> WarningLog { get; } = new();
    
    public void Info(string message)
    {
        InfoLog.Add(message);
    }

    public void Warning(string message)
    {
        WarningLog.Add(message);
    }

    public void Error(string message, Exception ex)
    {
        ErrorLog.Add(message);
    }
}