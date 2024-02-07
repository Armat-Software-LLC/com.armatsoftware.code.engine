using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Storage;

public interface IStoredSubjectAction<T> : ICollection<IStoredActionRevision<T>>, ISubjectAction<T>
    where T : class
{
    /// <summary>
    /// Update the action, thereby creating a new latest revision
    /// </summary>
    /// <param name="code"></param>
    /// <param name="author"></param>
    /// <param name="comment"></param>
    void Update(string code, string author, string comment);
    
    /// <summary>
    /// Mark a revision as active
    /// </summary>
    /// <param name="revision"></param>
    void Activate(int revision);
}