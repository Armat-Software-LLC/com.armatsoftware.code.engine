using System;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.Execution;

public class ExecutorCatalog<TSubject>(IExecutorFactory factory) : IExecutorCatalog<TSubject>
    where TSubject : class, new()
{
    private readonly IExecutorFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    public IExecutor<TSubject> ForKey(string key)
    {
        return _factory.Provide<TSubject>(key);
    }
}