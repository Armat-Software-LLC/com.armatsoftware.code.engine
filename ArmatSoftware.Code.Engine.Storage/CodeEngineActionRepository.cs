using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Configuration;

namespace ArmatSoftware.Code.Engine.Storage;

public class CodeEngineActionRepository : IActionRepository
{
    private readonly IConfiguration _configuration;
    private readonly ICodeEngineLogger _logger;
    private readonly IStorageAdapter _storageAdapter;
    
    public CodeEngineActionRepository(IConfiguration configuration, ICodeEngineLogger logger, IStorageAdapter storageAdapter)
    {
        _configuration = configuration ??
                         throw new ArgumentNullException(nameof(configuration), "Supplied configuration is null");
        
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");

        _storageAdapter =
            storageAdapter ??
            throw new ArgumentNullException(nameof(storageAdapter), "Supplied storage adapter is null"); // new FileIOAdapter(fileStorageRootPath, fileExtension, _logger);
    }

    public IEnumerable<ISubjectAction<TSubject>> GetActions<TSubject>(string key = "") where TSubject : class
    {
        var storedActions = _storageAdapter.Read<TSubject>(key);
        return storedActions;
    }

    public void AddAction<TSubject>(string name, string code, string author, string comment, string key = "") where TSubject : class
    {
        var actions = _storageAdapter.Read<TSubject>(key);
        var newAction = actions.Create(name);
        newAction.Update(code, author, comment);
        newAction.Activate(1);
        _storageAdapter.Write(actions, key);
    }

    public void UpdateAction<TSubject>(string name, string code, string author, string comment, string key = "") where TSubject : class
    {
        var actions = _storageAdapter.Read<TSubject>(key);
        var action = actions.First(a => a.Name == name);
        action.Update(code, author, comment);
        _storageAdapter.Write(actions, key);
    }

    public void ActivateRevision<TSubject>(string actionName, int revision, string key = "") where TSubject : class
    {
        var actions = _storageAdapter.Read<TSubject>(key);
        var action = actions.First(a => a.Name == actionName);
        action.Activate(revision);
        _storageAdapter.Write(actions, key);
    }

    public void ReorderAction<TSubject>(string actionName, int newOrder, string key = "") where TSubject : class
    {
        var actions = _storageAdapter.Read<TSubject>(key);
        actions.Reorder(actionName, newOrder);
        _storageAdapter.Write(actions, key);
    }
}