using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Configuration;

namespace ArmatSoftware.Code.Engine.Storage;

public class CodeEngineActionProvider : IActionProvider
{
    public const string FileStoragePath = "ASCE_FILE_STORAGE_PATH";
    public const string FileStorageExtension = "ASCE_FILE_STORAGE_EXTENSION";

    private readonly IConfiguration _configuration;
    private readonly ICodeEngineLogger _logger;
    private readonly IStorageAdapter  _fileIOAdapter;
    
    public CodeEngineActionProvider(IConfiguration configuration, ICodeEngineLogger logger, IStorageAdapter storageAdapter)
    {
        _configuration = configuration ??
                         throw new ArgumentNullException(nameof(configuration), "Supplied configuration is null");
        
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");
        
        _fileIOAdapter =
            storageAdapter ??
            throw new ArgumentNullException(nameof(storageAdapter), "Supplied storage adapter is null"); // new FileIOAdapter(fileStorageRootPath, fileExtension, _logger);
    }

    public IEnumerable<ISubjectAction<TSubject>> Retrieve<TSubject>(string key = "") where TSubject : class
    {
        _logger.Info($"Retrieving stored actions for subject type {typeof(TSubject).FullName} and key '{key}'.");
        
        // Try to retrieve stored actions for the given key
        var storedActions = _fileIOAdapter.Read<TSubject>(key);
        
        // If no stored actions are found for the given key, try to retrieve default actions
        if (!storedActions.Any() && !string.IsNullOrEmpty(key))
        {
            _logger.Info($"No stored actions found for {typeof(TSubject).FullName} and key '{key}'. Trying to retrieve default actions");
            storedActions = _fileIOAdapter.Read<TSubject>();
        }
        
        return storedActions;
    }
}
