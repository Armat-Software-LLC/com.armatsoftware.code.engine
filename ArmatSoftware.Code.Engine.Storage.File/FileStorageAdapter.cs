using System.Security;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class FileStorageAdapter : IStorageAdapter
{
    private readonly DirectoryInfo _storageRootDirectory;
    private readonly string _fileExtension;
    private readonly ILogger _logger;

    public FileStorageAdapter(FileStorageOptions options, ILogger logger)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options), "Supplied options are null");

        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");
        
        if (string.IsNullOrWhiteSpace(options.StoragePath))
        {
            throw new ArgumentNullException("Supplied storage path is empty or null", nameof(options.StoragePath));
        }
        
        if (string.IsNullOrWhiteSpace(options.FileExtension))
        {
            throw new ArgumentNullException(nameof(options.FileExtension), "Supplied file extension is empty or null");
        }

        _fileExtension = options.FileExtension;
        
        try
        {
            _storageRootDirectory = new DirectoryInfo(options.StoragePath);

            if (!_storageRootDirectory.Exists)
            {
                _storageRootDirectory.Create();
            }
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError("Supplied storage path is empty or null", e);
            throw;
        }
        catch (ArgumentException e)
        {
            _logger.LogError("Supplied storage path is invalid", e);
            throw;
        }
        catch (SecurityException e)
        {
            _logger.LogError("Insufficient permissions to the supplied storage path", e);
            throw;
        }
        catch (PathTooLongException e)
        {
            _logger.LogError("Supplied storage path is too long", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("Exception occured", e);
            throw;
        }

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

    public IStoredSubjectActions<TSubject> Read<TSubject>(string key = "") where TSubject : class
    {
        var pathInfo = GeneratePath(typeof(TSubject), key);
        if (!System.IO.File.Exists(pathInfo.ToString()))
        {
            _logger.LogInformation($"Code file for {typeof(TSubject).FullName} and key '{key}' does not exist. Returning empty stored actions.");
            return new StoredSubjectActions<TSubject>();
        }
        
        var content = System.IO.File.ReadAllText(pathInfo.ToString());
        
        var storedActions = JsonConvert.DeserializeObject<StoredSubjectActions<TSubject>>(content);
        
        if (storedActions == null)
        {
            _logger.LogInformation($"Code file for {typeof(TSubject).FullName} and key '{key}' is empty. Returning empty stored actions.");
            return new StoredSubjectActions<TSubject>();
        }

        return storedActions;
    }

    public void Write<TSubject>(IStoredSubjectActions<TSubject> actions, string key = "") where TSubject : class
    {
        var pathInfo = GeneratePath(typeof(TSubject), key);
        
        if (!Directory.Exists(pathInfo.DirectoryPath))
        {
            Directory.CreateDirectory(pathInfo.DirectoryPath);
        }
        
        var content = JsonConvert.SerializeObject(actions);
        System.IO.File.WriteAllText(pathInfo.ToString(), content);
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