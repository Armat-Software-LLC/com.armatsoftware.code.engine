namespace ArmatSoftware.Code.Engine.Storage.File;

public class FileStorageOptions
{
    /// <summary>
    /// Root path of the storage location
    /// </summary>
    public string StoragePath { get; set; }
    
    /// <summary>
    /// File extension of the code files
    /// </summary>
    public string FileExtension { get; set; }
}