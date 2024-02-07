namespace ArmatSoftware.Code.Engine.Storage;

/// <summary>
/// Repository streamlines working with stored actions
/// </summary>
public interface IActionRepository
{
    /// <summary>
    /// Store the entire set of actions
    /// </summary>
    /// <param name="actions"></param>
    /// <typeparam name="T"></typeparam>
    void Store <T>(IStoredActions<T> actions) where T : class;
    
    /// <summary>
    /// retrieve the entire set of actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IStoredActions<T> Retrieve<T>() where T : class;
    
    /// <summary>
    /// Add a new action for subject type T
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IStoredSubjectAction<T> AddAction<T>(string name) where T : class;
    
    /// <summary>
    /// Update an existing action for subject of type T
    /// </summary>
    /// <param name="code"></param>
    /// <param name="author"></param>
    /// <param name="comment"></param>
    /// <typeparam name="T"></typeparam>
    void UpdateAction<T>(string name, string code, string author, string comment) where T : class;
    
    /// <summary>
    /// Activate a specific revision on the named action for subject of type T
    /// </summary>
    /// <param name="revision"></param>
    /// <typeparam name="T"></typeparam>
    void ActivateRevision<T>(string actionName, int revision) where T : class;
}