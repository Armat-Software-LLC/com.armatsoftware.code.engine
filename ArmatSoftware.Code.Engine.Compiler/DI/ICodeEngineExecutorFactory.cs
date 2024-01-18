using System;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler.DI
{
    public interface ICodeEngineExecutorFactory : IDisposable
    {
        IExecutor<T> Provide<T>()
            where T : class, new();
    }
}