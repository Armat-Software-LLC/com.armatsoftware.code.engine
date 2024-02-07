using ArmatSoftware.Code.Engine.Core.Logging;
using Microsoft.Extensions.Configuration;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class CodeEngineActionRepository : IActionRepository
{
    public const string FileStoragePath = "ASCE_FILE_STORAGE_PATH";
    public const string FileStorageExtension = "ASCE_FILE_STORAGE_EXTENSION";

    private readonly IConfiguration _configuration;
    private readonly ICodeEngineLogger _logger;
    private readonly FileIOAdapter _fileIOAdapter;
    
    public CodeEngineActionRepository(IConfiguration configuration, ICodeEngineLogger logger)
    {
        _configuration = configuration ??
                         throw new ArgumentNullException(nameof(configuration), "Supplied configuration is null");
        
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");

        var fileExtension = _configuration[FileStorageExtension] ?? "asce";

        var fileStorageRootPath = _configuration[FileStoragePath] ??
                                  throw new ApplicationException("File storage path is not configured");

        _fileIOAdapter = new FileIOAdapter(fileStorageRootPath, fileExtension, _logger);
    }
    
    public void Store<T>(IStoredActions<T> actions) where T : class
    {
        _logger.Info("Storing actions");
        _fileIOAdapter.Write(actions);
    }

    public IStoredActions<T> Retrieve<T>() where T : class
    {
        return _fileIOAdapter.Read<T>();
    }

    public IStoredSubjectAction<T> AddAction<T>(string name) where T : class
    {
        var actions = _fileIOAdapter.Read<T>();
        var newAction = actions.Add(name);
        _fileIOAdapter.Write(actions);
        return newAction;
    }

    public void UpdateAction<T>(string name, string code, string author, string comment) where T : class
    {
        var actions = _fileIOAdapter.Read<T>();
        var action = actions.First(a => a.Name == name);
        action.Update(code, author, comment);
        _fileIOAdapter.Write(actions);
    }

    public void ActivateRevision<T>(string actionName, int revision) where T : class
    {
        throw new NotImplementedException();
    }
}