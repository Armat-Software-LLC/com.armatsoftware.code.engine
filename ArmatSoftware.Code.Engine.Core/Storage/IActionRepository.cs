using System.Collections.Generic;

namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// Repository streamlines management of stored actions.
/// Use <c>ArmatSoftware.Code.Engine.Storage.File.DI.CodeEngineFileStorageRegistration.UseCodeEngineFileStorage()</c> to register
/// OOB implementation or provide your own.
/// </summary>
public interface IActionRepository
{
   
    /// <summary>
    /// retrieve the entire set of actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<ISubjectAction<T>> GetActions<T>() where T : class;
    
    /// <summary>
    /// Add a new action for subject type T
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    void AddAction<T>(string name, string code, string author, string comment) where T : class;
    
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
    void ActivateRevision<T>(string name, int revision) where T : class;
    
    /// <summary>
    /// Change the order of execution of the actions
    /// </summary>
    /// <param name="name"></param>
    /// <param name="newOrder"></param>
    /// <typeparam name="T"></typeparam>
    void ReorderAction<T>(string actionName, int newOrder) where T : class;
}