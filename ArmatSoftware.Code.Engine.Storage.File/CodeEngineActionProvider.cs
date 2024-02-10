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

    public IEnumerable<ISubjectAction<T>> Retrieve<T>() where T : class
    {
        _logger.Info("Retrieving stored actions");
        var storedActions = _fileIOAdapter.Read<T>();
        
        return storedActions;
    }
}
