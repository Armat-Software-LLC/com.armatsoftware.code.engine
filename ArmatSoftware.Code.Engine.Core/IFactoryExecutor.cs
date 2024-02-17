using ArmatSoftware.Code.Engine.Core.Logging;

namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// Use this interface to initialize an instance of IExecutor<> with a subject and logger
/// before it is returned from the factory
/// </summary>
/// <typeparam name="TSubject"></typeparam>
public interface IFactoryExecutor<TSubject> : IExecutor<TSubject>
    where TSubject : class
{
    void SetLogger(ICodeEngineLogger logger);
    
    /// <summary>
    /// Efficient cloning of the executors allows effective thread safe execution
    /// without using singletons or activating new instances
    /// </summary>
    /// <returns></returns>
    IFactoryExecutor<TSubject> Clone();
}