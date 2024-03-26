using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Compiler.Execution;
using ArmatSoftware.Code.Engine.Compiler.Tracing;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core.Tracing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
    
    protected IExecutorCache Cache { get; set; }
    
    protected ITracer Tracer { get; set; }
    
    protected Mock<IActionProvider> StorageMock { get; private set; }
    protected IActionProvider Provider { get; set; }
    
    protected CodeEngineExecutorLogTestLogger Logger { get; set; }
    
    protected IMemoryCache MemCache { get; set; }

    protected List<TestSubjectAction<TestSubject>> LogSubjectActions { get; set; }
    
    protected List<TestSubjectAction<TestSubject>> SetLogNullSubjectActions { get; set; }
    
    protected ExecutorFactory Factory { get; set; }

    [SetUp]
    public void Setup()
    {
        LogSubjectActions = new List<TestSubjectAction<TestSubject>>
        {
            new TestSubjectAction<TestSubject>()
            {
                Name = "LogInfo",
                Code = $"Log.LogInformation(\"{InfoMessage}\");",
                Order = 1
            },
            new TestSubjectAction<TestSubject>()
            {
                Name = "LogWarning",
                Code = $"Log.LogWarning(\"{WarningMessage}\");",
                Order = 2
            },
            new TestSubjectAction<TestSubject>()
            {
                Name = "LogError",
                Code = $"Log.LogError(\"{ErrorMessage}\");",
                Order = 3
            }
        };

        SetLogNullSubjectActions = new List<TestSubjectAction<TestSubject>>
        {
            new TestSubjectAction<TestSubject>()
            {
                Name = "SetLogNull",
                Code = "Log = null;",
                Order = 1
            }
        };
    }
    
    protected IExecutor<TestSubject> BuildLogActions(List<TestSubjectAction<TestSubject>> subjectActions)
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
        
        Cache = new ExecutorCache(MemCache, RegistrationOptions);
        
        Tracer = new Tracer();
        
        Factory = new ExecutorFactory(RegistrationOptions, Provider, Cache, Tracer, Logger);
        
        return Factory.Provide<TestSubject>();
    }
}

public class CodeEngineExecutorLogTestLogger : ILogger
{
    public List<string> InfoLog { get; } = new();
    public List<string> ErrorLog { get; } = new();
    public List<string> WarningLog { get; } = new();
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        switch (logLevel)
        {
            case LogLevel.Information:
                InfoLog.Add(state.ToString());
                break;
            case LogLevel.Warning:
                WarningLog.Add(state.ToString());
                break;
            case LogLevel.Error:
                ErrorLog.Add(state.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}