using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Storage;

/// <summary>
/// Default implementation of the action provider for the code engine, if this package is used.
/// </summary>
public class CodeEngineActionProvider(ILogger logger, IStorageAdapter storageAdapter) : IActionProvider
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");
    private readonly IStorageAdapter  _storageAdapter = storageAdapter ??
                                                        throw new ArgumentNullException(nameof(storageAdapter), "Supplied storage adapter is null"); // new FileIOAdapter(fileStorageRootPath, fileExtension, _logger);

    public IEnumerable<ISubjectAction<TSubject>> Retrieve<TSubject>(string key = "") where TSubject : class
    {
        _logger.LogInformation($"Retrieving stored actions for subject type {typeof(TSubject).FullName} and key '{key}'.");
        
        // Try to retrieve stored actions for the given key
        var storedActions = _storageAdapter.Read<TSubject>(key);
        
        // If no stored actions are found for the given key, try to retrieve default actions
        if (!storedActions.Any() && !string.IsNullOrEmpty(key))
        {
            _logger.LogInformation($"No stored actions found for {typeof(TSubject).FullName} and key '{key}'. Trying to retrieve default actions");
            storedActions = _storageAdapter.Read<TSubject>();
        }
        
        return storedActions;
    }
}
