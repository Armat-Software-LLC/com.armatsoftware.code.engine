using System.Security;
using System.Xml.Serialization;
using ArmatSoftware.Code.Engine.Core.Logging;

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
    
    public IStoredActions<T> Read<T>()
        where T : class
    {
        var pathInfo = GeneratePath(typeof(T));
        if (!System.IO.File.Exists(pathInfo.ToString()))
        {
            _logger.Info($"Code file for {typeof(T).FullName} does not exist. Returning empty stored actions.");
            return new StoredActions<T>();
        }
        
        using (var fileStream = new FileStream(pathInfo.ToString(), FileMode.Open))
        {
            _logger.Info($"Reading code file for {typeof(T).FullName} from {pathInfo}.");
            var serializer = new XmlSerializer(typeof(T));
            return (IStoredActions<T>)serializer.Deserialize(fileStream);
        }
    }
    
    public void Write<T>(IStoredActions<T> actions)
        where T : class
    {
        var pathInfo = GeneratePath(typeof(T));
        using (var fileStream = new FileStream(pathInfo.ToString(), FileMode.Create))
        {
            _logger.Info($"Writing code file for {typeof(T).FullName} to {pathInfo}.");
            var serializer = new XmlSerializer(typeof(IStoredActions<T>));
            serializer.Serialize(fileStream, actions);
        }
    }
    
    private GeneratedPathInfo GeneratePath(Type subjectType)
    {
        var folderPath = subjectType.FullName?
                             .Replace("+", $"{Path.DirectorySeparatorChar}")
                             .Replace(".", $"{Path.DirectorySeparatorChar}") ?? // contained classes turn into subfolders
                         throw new ArgumentNullException(nameof(subjectType), "Supplied subject type is null");

        var fileName = $"code.{_fileExtension}";

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