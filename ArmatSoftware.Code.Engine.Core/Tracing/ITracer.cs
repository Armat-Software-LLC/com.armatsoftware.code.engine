namespace ArmatSoftware.Code.Engine.Core.Tracing;

/// <summary>
/// Provider of the correlation identifier to trace execution of multiple code engine components
/// within a single user web request.
/// </summary>
public interface ITracer
{
    /// <summary>
    /// Correlation identifier that persists across multiple code engine components within a single user web request.
    /// </summary>
    string CorrelationId { get; }
}