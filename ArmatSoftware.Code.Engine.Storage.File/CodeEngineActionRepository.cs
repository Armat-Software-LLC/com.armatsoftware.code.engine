using ArmatSoftware.Code.Engine.Core;
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

    public IEnumerable<ISubjectAction<TSubject>> GetActions<TSubject>(string key = "") where TSubject : class
    {
        var storedActions = _fileIOAdapter.Read<TSubject>(key);
        return storedActions;
    }

    public void AddAction<TSubject>(string name, string code, string author, string comment, string key = "") where TSubject : class
    {
        var actions = _fileIOAdapter.Read<TSubject>(key);
        var newAction = actions.Add(name);
        newAction.Update(code, author, comment);
        newAction.Activate(1);
        _fileIOAdapter.Write(actions, key);
    }

    public void UpdateAction<TSubject>(string name, string code, string author, string comment, string key = "") where TSubject : class
    {
        var actions = _fileIOAdapter.Read<TSubject>(key);
        var action = actions.First(a => a.Name == name);
        action.Update(code, author, comment);
        _fileIOAdapter.Write(actions, key);
    }

    public void ActivateRevision<TSubject>(string actionName, int revision, string key = "") where TSubject : class
    {
        var actions = _fileIOAdapter.Read<TSubject>(key);
        var action = actions.First(a => a.Name == actionName);
        action.Activate(revision);
        _fileIOAdapter.Write(actions, key);
    }

    public void ReorderAction<TSubject>(string actionName, int newOrder, string key = "") where TSubject : class
    {
        var actions = _fileIOAdapter.Read<TSubject>(key);
        actions.Reorder(actionName, newOrder);
        _fileIOAdapter.Write(actions, key);
    }
}