using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage;
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
        var executor = BuildLogActions(LogSubjectActions);

        executor.Execute(new TestSubject());
        
        Assert.That(Logger.InfoLog.Any(x => x == InfoMessage));
    }
    
    [Test]
    public void Should_Log_Error_Message()
    {
        var executor = BuildLogActions(LogSubjectActions);
        
        executor.Execute(new TestSubject());
        
        Assert.That(Logger.ErrorLog.Any(x => x == ErrorMessage));
    }
    
    [Test]
    public void Should_Log_Warning_Message()
    {
        var executor = BuildLogActions(LogSubjectActions);
        
        executor.Execute(new TestSubject());
        
        Assert.That(Logger.WarningLog.Any(x => x == WarningMessage));
    }
    
    [Test]
    public void Should_Not_Set_Log()
    {
        Assert.That(() =>
        {
            var executor = BuildLogActions(SetLogNullSubjectActions);
        
            executor.Execute(new TestSubject());

        }, Throws.InvalidOperationException);
    }
}


public class CodeEngineExecutorLogTestBuilder
{
    protected const string InfoMessage = "info message";
    protected const string ErrorMessage = "error message";
    protected const string WarningMessage = "warning message";
    
    protected CodeEngineOptions RegistrationOptions { get; set; }
    
    protected ICodeEngineExecutorCache Cache { get; set; }
    
    protected Mock<IActionProvider> StorageMock { get; private set; }
    protected IActionProvider Provider { get; set; }
    
    protected CodeEngineExecutorLogTestLogger Logger { get; set; }
    
    protected IMemoryCache MemCache { get; set; }

    protected StoredSubjectActions<TestSubject> LogSubjectActions { get; set; }
    
    protected StoredSubjectActions<TestSubject> SetLogNullSubjectActions { get; set; }
    
    protected CodeEngineExecutorFactory Factory { get; set; }

    [SetUp]
    public void Setup()
    {
        LogSubjectActions = new StoredSubjectActions<TestSubject>();
        
        var logInfoAction = LogSubjectActions.Create("LogInfo");
        logInfoAction.Update($"Log.Info(\"{InfoMessage}\");", "testauthor", "testcomment");
        logInfoAction.Activate(1);
        
        var logWarningAction = LogSubjectActions.Create("LogWarning");
        logWarningAction.Update($"Log.Warning(\"{WarningMessage}\");", "testauthor", "testcomment");
        logWarningAction.Activate(1);
        
        var logErrorAction = LogSubjectActions.Create("LogError");
        logErrorAction.Update($"Log.Error(\"{ErrorMessage}\");", "testauthor", "testcomment");
        logErrorAction.Activate(1);

        SetLogNullSubjectActions = new StoredSubjectActions<TestSubject>();
        
        var setLogNullAction = SetLogNullSubjectActions.Create("SetLogNull");
        setLogNullAction.Update($"Log = null;", "testauthor", "testcomment");
        setLogNullAction.Activate(1);
        
    }
    
    protected IExecutor<TestSubject> BuildLogActions(StoredSubjectActions<TestSubject> subjectActions)
    {
        StorageMock = new Mock<IActionProvider>();
        StorageMock.Setup(x => x.Retrieve<TestSubject>(It.IsAny<string>()))
            .Returns(subjectActions.ToList());

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