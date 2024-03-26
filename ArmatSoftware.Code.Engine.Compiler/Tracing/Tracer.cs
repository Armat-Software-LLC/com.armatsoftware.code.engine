using System;
using ArmatSoftware.Code.Engine.Core.Tracing;

namespace ArmatSoftware.Code.Engine.Compiler.Tracing;

/// <summary>
/// Default implementation of the <c>ITracer</c> interface with a simple responsibility
/// to generate new correlation id for each instance of the tracer.
/// </summary>
public class Tracer : ITracer
{
    public string CorrelationId { get; } = Guid.NewGuid().ToString();
}