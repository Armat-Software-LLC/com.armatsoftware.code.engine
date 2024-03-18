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
using Microsoft.Extensions.Options;
using Moq;

namespace ArmatSoftware.Code.Engine.Tests.Unit;

internal class ExecutorBuilderBase<TSubject>
    where TSubject : class, new()
{
    protected CodeEngineOptions RegistrationOptions { get; set; }
    
    protected IExecutorCache Cache { get; set; }
    
    protected ITracer Tracer { get; set; }
    
    protected Mock<IActionProvider> StorageMock { get; private set; }
    protected IActionProvider Provider { get; set; }
    
    protected CodeEngineExecutorLogTestLogger Logger { get; set; }
    
    protected IMemoryCache MemCache { get; set; }
    
    protected ExecutorFactory Factory { get; set; }


    public void Init()
    {
        StorageMock = new Mock<IActionProvider>();
        
        Logger = new CodeEngineExecutorLogTestLogger();
        
        RegistrationOptions = new CodeEngineOptions
        {
            CodeEngineNamespace = "ArmatSoftware.Code.Engine.Tests.Unit",
            Provider = Provider,
            Logger = Logger,
            Tracer = new Tracer()
        };
        
        MemCache = new MemoryCache(Options.Create(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromMinutes(1)
        }));
        
        Cache = new ExecutorCache(MemCache, RegistrationOptions);
        
        Tracer = new Tracer();
    }
    
    protected IExecutor<TSubject> BuildAndCompile(IEnumerable<ISubjectAction<TSubject>> subjectActions)
    {
        StorageMock.Setup(x => x.Retrieve<TSubject>(It.IsAny<string>()))
            .Returns(subjectActions.ToList());
        Provider = StorageMock.Object;
        Factory = new ExecutorFactory(RegistrationOptions, Provider, Cache, Tracer);
        
        return Factory.Provide<TSubject>();
    }
}