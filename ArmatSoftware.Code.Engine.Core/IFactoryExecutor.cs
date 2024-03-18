using ArmatSoftware.Code.Engine.Core.Logging;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// Use this interface to initialize an instance of IExecutor<> with a logger
/// before it is returned from the factory
/// </summary>
/// <typeparam name="TSubject"></typeparam>
public interface IFactoryExecutor<TSubject> : IExecutor<TSubject>
    where TSubject : class
{
    void SetLogger(ILogger logger, LogContext context);
}