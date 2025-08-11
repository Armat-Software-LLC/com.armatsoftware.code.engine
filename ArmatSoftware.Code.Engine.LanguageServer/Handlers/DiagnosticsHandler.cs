using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ArmatSoftware.Code.Engine.LanguageServer.Services;

namespace ArmatSoftware.Code.Engine.LanguageServer.Handlers;

public class DiagnosticsHandler
{
    private readonly IDiagnosticsService _diagnosticsService;

    public DiagnosticsHandler(IDiagnosticsService diagnosticsService)
    {
        _diagnosticsService = diagnosticsService;
    }

    public async Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(string content, string uri, Type? subjectType = null)
    {
        return await _diagnosticsService.GetDiagnosticsAsync(content, uri, subjectType);
    }
}