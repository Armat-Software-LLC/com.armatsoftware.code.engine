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
        
        Assert.That(InfoLog.Any(x => x == InfoMessage));
    }
    
    [Test]
    public void Should_Log_Error_Message()
    {
        var executor = Build();
        
        executor.Execute(new TestSubject());
        
        Assert.That(ErrorLog.Any(x => x == ErrorMessage));
    }
    
    [Test]
    public void Should_Log_Warning_Message()
    {
        var executor = Build();
        
        executor.Execute(new TestSubject());
        
        Assert.That(WarningLog.Any(x => x == WarningMessage));
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
    protected ICodeEngineLogger Logger { get; set; }
    
    protected IMemoryCache MemCache { get; set; }

    protected StoredActions<CodeEngineFactoryTestSubject> Actions { get; set; }
    
    
    protected CodeEngineExecutorFactory Factory { get; set; }
    
    protected List<string> InfoLog { get; set; }
    protected List<string> ErrorLog { get; set; }
    protected List<string> WarningLog { get; set; }

    [SetUp]
    public void Setup()
    {
        Actions = new StoredActions<CodeEngineFactoryTestSubject>();
        var action = Actions.Add("LogAction");
        action.Update($"Log.Info(\"{InfoMessage}\");", "testauthor", "testcomment");
        action.Activate(1);
        
        StorageMock = new Mock<IActionProvider>();
        StorageMock.Setup(x => x.Retrieve<CodeEngineFactoryTestSubject>(It.IsAny<string>()))
            .Returns(Actions.ToList());
        LoggerMock = new Mock<ICodeEngineLogger>();
        LoggerMock.Setup(x => x.Info(It.IsAny<string>())).Callback(() => InfoLog.Add(InfoMessage));
        LoggerMock.Setup(x => x.Warning(It.IsAny<string>())).Callback(() => WarningLog.Add(WarningMessage));
        LoggerMock.Setup(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>())).Callback(() => ErrorLog.Add(ErrorMessage));
        
        Provider = StorageMock.Object;
        Logger = LoggerMock.Object;
        
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
        
        InfoLog = new List<string>();
        ErrorLog = new List<string>();
        WarningLog = new List<string>();
    }
    
    protected IExecutor<TestSubject> Build()
    {
        return Factory.Provide<TestSubject>();
    }
}