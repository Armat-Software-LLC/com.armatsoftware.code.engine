namespace ArmatSoftware.Code.Engine.Storage;

/// <summary>
/// Management interface for stored actions
/// </summary>
public interface ICodeEngineActionStorage
{
    // create new action
    public IStoredSubjectAction<T> Create<T>(string name)
        where T : class;

    // update existing action
    public void Update<T>(string code, string author, string comment)
        where T : class;
    
    // delete action
    public void Delete<T>()
        where T : class;
    
    // get action
    public IStoredActions<T> Get<T>(string name)
        where T : class;
    
    // reorder actions
    public void Reorder<T>(string name, int order)
        where T : class;
}