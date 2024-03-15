using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Core.Logging;

public struct LogMessage
{
    public LogContext Context { get; set; }
    
    public string Message { get; set; }
    
    public LogLevel Level { get; set; }
}