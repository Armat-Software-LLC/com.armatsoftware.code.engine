namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// Indexed executor interface for the code engine framework.
/// </summary>
/// <typeparam name="TSubject">Subject type</typeparam>
public interface IExecutorCatalog<TSubject>
    where TSubject : class, new()
{
    IExecutor<TSubject> ForKey(string key);
}