using System.Security;
using System.Xml.Serialization;
using ArmatSoftware.Code.Engine.Core.Logging;
using Newtonsoft.Json;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class FileIOAdapter
{
    private readonly DirectoryInfo _storageRootDirectory;
    private readonly string _fileExtension;
    private readonly ICodeEngineLogger _logger;

    public FileIOAdapter(string storageRootPath, string fileExtension, ICodeEngineLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");

        _fileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension), "Supplied file extension is null");
        
        try
        {
            _storageRootDirectory = new DirectoryInfo(storageRootPath);

            if (!_storageRootDirectory.Exists)
            {
                _storageRootDirectory.Create();
            }
        }
        catch (ArgumentNullException e)
        {
            _logger.Error("Supplied storage path is empty or null", e);
            throw;
        }
        catch (ArgumentException e)
        {
            _logger.Error("Supplied storage path is invalid", e);
            throw;
        }
        catch (SecurityException e)
        {
            _logger.Error("Insufficient permissions to the supplied storage path", e);
            throw;
        }
        catch (PathTooLongException e)
        {
            _logger.Error("Supplied storage path is too long", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.Error("Exception occured", e);
            throw;
        }

    }
    
    public StoredActions<TSubject> Read<TSubject>(string key = "")
        where TSubject : class
    {
        var pathInfo = GeneratePath(typeof(TSubject), key);
        if (!System.IO.File.Exists(pathInfo.ToString()))
        {
            _logger.Info($"Code file for {typeof(TSubject).FullName} and key '{key}' does not exist. Returning empty stored actions.");
            return new StoredActions<TSubject>();
        }
        
        var content = System.IO.File.ReadAllText(pathInfo.ToString());
        return JsonConvert.DeserializeObject<StoredActions<TSubject>>(content) ?? new StoredActions<TSubject>();
    }
    
    public void Write<TSubject>(StoredActions<TSubject> actions, string key = "")
        where TSubject : class
    {
        var pathInfo = GeneratePath(typeof(TSubject), key);
        
        if (!Directory.Exists(pathInfo.DirectoryPath))
        {
            Directory.CreateDirectory(pathInfo.DirectoryPath);
        }
        
        var content = JsonConvert.SerializeObject(actions);
        System.IO.File.WriteAllText(pathInfo.ToString(), content);
    }
    
    /// <summary>
    /// Generate unique path for the code file based on the subject type and key
    /// </summary>
    /// <param name="subjectType"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private GeneratedPathInfo GeneratePath(Type subjectType, string key = "")
    {
        var folderPath = subjectType.FullName?
                             .Replace("+", $"{Path.DirectorySeparatorChar}")
                             .Replace(".", $"{Path.DirectorySeparatorChar}") ?? // contained classes turn into subfolders
                         throw new ArgumentNullException(nameof(subjectType), "Supplied subject type is null");
        
        var validKey = key ?? throw new ArgumentNullException(nameof(key), "Supplied key is null!");

        var keyFragment = validKey.Length > 0 ? $".{validKey}" : string.Empty;
        
        var fileName = $"code{keyFragment}.{_fileExtension}";

        return new GeneratedPathInfo(Path.Join(_storageRootDirectory.FullName, folderPath), fileName);
    }
}

public class GeneratedPathInfo(string directoryPath, string codeFileName)
{
    public string DirectoryPath { get; private set; } = directoryPath;
    public string CodeFileName { get; private set; } = codeFileName;

    public override string ToString()
    {
        return $"{DirectoryPath}{Path.DirectorySeparatorChar}{CodeFileName}";
    }
}