using System.Security;
using System.Text;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using Microsoft.Extensions.Configuration;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class CodeEngineFileStorage : ICodeEngineStorage
{
    public const string FileStoragePath = "ASCE_FILE_STORAGE_PATH";
    public const string FileStorageExtension = "ASCE_FILE_STORAGE_EXTENSION";

    private readonly IConfiguration _configuration;
    private readonly ICodeEngineLogger _logger;
    private readonly DirectoryInfo _storageRootPath;
    private readonly string _fileExtension;
    
    //TODO: apply singleton pattern?
    public CodeEngineFileStorage(IConfiguration configuration, ICodeEngineLogger logger)
    {
        _configuration = configuration ??
                         throw new ArgumentNullException(nameof(configuration), "Supplied configuration is null");
        
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Supplied logger is null");

        _fileExtension = _configuration[FileStorageExtension] ?? "code";
        
        var fileStorageConfiguration = _configuration[FileStoragePath]; // .GetSection(FileStoragePath);
        if (fileStorageConfiguration == null)
        {
            throw new ApplicationException("File storage configuration is null or invalid");
        }

        try
        {
            _storageRootPath = new DirectoryInfo(fileStorageConfiguration);

            if (!_storageRootPath.Exists)
            {
                _storageRootPath.Create();
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
            Console.WriteLine(e);
            throw;
        }
    }

    public string Retrieve(Type subjectType, Guid executorId)
    {
        var fullPath = GeneratePath(subjectType, executorId);
        var codeFile = _storageRootPath.GetFiles(fullPath).First();
        var code = codeFile.OpenText().ReadToEnd();
        return code;
    }

    public void Store(Type subjectType, Guid executorId, string code)
    {
        var fullPath = GeneratePath(subjectType, executorId);
        var codeFile = _storageRootPath.GetFiles(fullPath).First();
        var stream = codeFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        byte[] bytes = new UTF8Encoding(true).GetBytes(code);
        stream.Write(bytes, 0, bytes.Length);
    }

    private string GeneratePath(Type subjectType, Guid executorId)
    {
        //TODO: add replace for each non-path character
        var folderPath = subjectType.FullName?
                             .Replace("+", "_") ?? 
                         throw new ArgumentNullException(nameof(subjectType), "Supplied subject type is null");

        var fileName = $"{executorId.ToString()}.{_fileExtension}";

        return Path.Join(_storageRootPath.FullName, folderPath, fileName);
    }
}
