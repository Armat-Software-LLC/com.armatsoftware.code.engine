namespace ArmatSoftware.Code.Engine.Core.Logging;

public struct LogContext
{
    /// <summary>
    /// Optional key used to retrieve executor
    /// </summary>
    public string ExecutorKey { get; set; }
    
    /// <summary>
    /// Subject type
    /// </summary>
    public string SubjectType { get; set; }
    
    /// <summary>
    /// Name of the generated executor class
    /// </summary>
    public string ExecutorName { get; set; }

    /// <summary>
    /// Name of the action where the log message was generated
    /// </summary>
    public string ExecutorAction { get; set; }
}