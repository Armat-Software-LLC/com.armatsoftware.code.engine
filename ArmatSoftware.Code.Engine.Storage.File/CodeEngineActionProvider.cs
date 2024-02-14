using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using Microsoft.Extensions.Configuration;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class CodeEngineActionProvider : IActionProvider
{
    public const string FileStoragePath = "ASCE_FILE_STORAGE_PATH";
    public const string FileStorageExtension = "ASCE_FILE_STORAGE_EXTENSION";

    private readonly IConfiguration _configuration;
    private readonly ICodeEngineLogger _logger;
    private readonly FileIOAdapter  _fileIOAdapter;
    
    public CodeEngineActionProvider(IConfiguration configuration, ICodeEngineLogger logger)
    {
        _configuration = configuration ??
                         throw new ArgumentNullException(nameof(configuration), "Supplied configuration is null");
        
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");

        var fileExtension = _configuration[FileStorageExtension] ?? "asce";

        var fileStorageRootPath = _configuration[FileStoragePath] ??
                                       throw new ApplicationException("File storage path is not configured");

        _fileIOAdapter = new FileIOAdapter(fileStorageRootPath, fileExtension, _logger);
    }

    public IEnumerable<ISubjectAction<T>> Retrieve<T>(string key = "") where T : class
    {
        _logger.Info($"Retrieving stored actions for subject type {typeof(T).FullName} and key '{key}'.");
        
        // Try to retrieve stored actions for the given key
        var storedActions = _fileIOAdapter.Read<T>(key);
        
        // If no stored actions are found for the given key, try to retrieve default actions
        if (!storedActions.Any() && !string.IsNullOrEmpty(key))
        {
            _logger.Info($"No stored actions found for {typeof(T).FullName} and key '{key}'. Trying to retrieve default actions");
            storedActions = _fileIOAdapter.Read<T>();
        }
        
        return storedActions;
    }
}
