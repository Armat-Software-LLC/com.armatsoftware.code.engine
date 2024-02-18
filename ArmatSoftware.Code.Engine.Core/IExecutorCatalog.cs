namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// Executor catalog for lookup by key.
/// </summary>
/// <typeparam name="TSubject">Subject type</typeparam>
public interface IExecutorCatalog<TSubject>
    where TSubject : class, new()
{
    IExecutor<TSubject> ForKey(string key);
}