namespace ArmatSoftware.Code.Engine.Storage;

/// <summary>
/// Streamlined storage of latest action revisions in an ordered list
/// according to the order of execution
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStoredActions<T> : ICollection<IStoredSubjectAction<T>>
    where T : class
{
    /// <summary>
    /// Add a new action to the list
    /// </summary>
    /// <param name="name">Name of the action</param>
    IStoredSubjectAction<T> Add(string name);

    /// <summary>
    /// Move an action to a new execution order
    /// </summary>
    /// <param name="name">Name of the action to move</param>
    /// <param name="order">New execution order</param>
    void Reorder(string name, int order);
}