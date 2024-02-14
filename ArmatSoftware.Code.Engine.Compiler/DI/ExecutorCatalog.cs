using System;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.DI;

public class ExecutorCatalog<TSubject> : IExecutorCatalog<TSubject>
    where TSubject : class, new()
{
    private readonly ICodeEngineExecutorFactory _factory;

    public ExecutorCatalog(ICodeEngineExecutorFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public IExecutor<TSubject> ForKey(string key)
    {
        return _factory.Provide<TSubject>(key);
    }
}