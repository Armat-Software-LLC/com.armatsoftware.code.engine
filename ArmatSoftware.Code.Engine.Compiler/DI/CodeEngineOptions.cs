using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Core.Tracing;
using Microsoft.Extensions.Logging;

namespace ArmatSoftware.Code.Engine.Compiler.DI;

/// <summary>
/// Pass to the <c>UseCodeEngine</c> method to configure the code engine.
/// Make sure to supply at least the <c>CompilerType</c> and <c>CodeEngineNamespace</c> properties.
/// </summary>
public class CodeEngineOptions
{
    /// <summary>
    /// Choose between C# and VB.NET compilers. This depends on the language of the code you use.
    /// </summary>
    public CompilerTypeEnum CompilerType { get; set; }
            
    /// <summary>
    /// Namespace where all generated executors will be placed.
    /// Make sure it is different from the namespace of the code you use.
    /// </summary>
    public string CodeEngineNamespace { get; set; }

    /// <summary>
    /// Optionally, provide a logger to use for the code engine.
    /// If none is provided, a <c>CodeEngineFileLogger</c> will be used.
    /// </summary>
    public ICodeEngineLogger Logger { get; set; }
            
    /// <summary>
    /// Optionally, provide a storage to use for the code engine.
    /// If none provided, a <c>CodeEngineFileStorage</c> will be used.
    /// </summary>
    public IActionProvider Provider { get; set; }
    
    /// <summary>
    /// Time in minutes before cached executors expire.
    /// </summary>
    public double CacheExpirationMinutes { get; set; } = 3;
    
    /// <summary>
    /// Data-annotated subject models will be validated before they are returned to the caller.
    /// </summary>
    public bool ValidateModelsAfterExecution { get; set; } = false;
    
    /// <summary>
    /// Provider of essential tracing information for the code engine.
    /// </summary>
    public ITracer Tracer { get; set; }
    
    /// <summary>
    /// Log level for the code engine. Use with default logger.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}