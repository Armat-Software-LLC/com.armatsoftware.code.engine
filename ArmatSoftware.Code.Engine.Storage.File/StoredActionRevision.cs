namespace ArmatSoftware.Code.Engine.Storage.File;

public record StoredActionRevision<T> : IStoredActionRevision<T>
    where T : class
{
    public int Revision { get; set; }
    public bool Active { get; set; }
    public string Code { get; set; }
    public string Hash { get; set; }
    public string Author { get; set; }
    public string Comment { get; set; }
    public DateTimeOffset Created { get; set; }
}