namespace ArmatSoftware.Code.Engine.Core.Tracing;

/// <summary>
/// Optional metadata assigned to generated executors before they are cloned.
/// </summary>
public interface IExecutorMetadata
{
    void SetMetadata(string key, string compiler);
}
