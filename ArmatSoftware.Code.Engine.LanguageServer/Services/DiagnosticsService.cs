using Microsoft.CodeAnalysis;
using ArmatSoftware.Code.Engine.LanguageServer.Services;
using LspDiagnostic = OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic;
using LspDiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using LspRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace ArmatSoftware.Code.Engine.LanguageServer.Services;

public interface IDiagnosticsService
{
    Task<IEnumerable<LspDiagnostic>> GetDiagnosticsAsync(string code, string uri, Type? subjectType = null);
}

public class DiagnosticsService : IDiagnosticsService
{
    private readonly ICodeEngineAnalysisService _analysisService;

    public DiagnosticsService(ICodeEngineAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    public async Task<IEnumerable<LspDiagnostic>> GetDiagnosticsAsync(string code, string uri, Type? subjectType = null)
    {
        var diagnostics = new List<LspDiagnostic>();

        // Get compilation diagnostics
        var compilationDiagnostics = await _analysisService.GetDiagnosticsAsync(code, subjectType);
        
        foreach (var diagnostic in compilationDiagnostics)
        {
            var lspDiagnostic = ConvertToLspDiagnostic(diagnostic);
            if (lspDiagnostic != null)
            {
                diagnostics.Add(lspDiagnostic);
            }
        }

        // Add Code Engine specific validations
        var codeEngineValidations = await ValidateCodeEngineRules(code, subjectType);
        diagnostics.AddRange(codeEngineValidations);

        return diagnostics;
    }

    private LspDiagnostic? ConvertToLspDiagnostic(Microsoft.CodeAnalysis.Diagnostic diagnostic)
    {
        if (!diagnostic.Location.IsInSource)
            return null;

        var lineSpan = diagnostic.Location.GetLineSpan();
        
        // Adjust line numbers to account for generated wrapper code
        var adjustedStart = AdjustPosition(lineSpan.StartLinePosition);
        var adjustedEnd = AdjustPosition(lineSpan.EndLinePosition);

        // Skip diagnostics that are in the generated wrapper code
        if (adjustedStart.Line < 0 || adjustedEnd.Line < 0)
            return null;

        return new LspDiagnostic
        {
            Range = new LspRange
            {
                Start = new Position(adjustedStart.Line, adjustedStart.Character),
                End = new Position(adjustedEnd.Line, adjustedEnd.Character)
            },
            Severity = ConvertSeverity(diagnostic.Severity),
            Code = diagnostic.Id,
            Message = diagnostic.GetMessage(),
            Source = "Code Engine"
        };
    }

    private Microsoft.CodeAnalysis.Text.LinePosition AdjustPosition(Microsoft.CodeAnalysis.Text.LinePosition position)
    {
        // The user code starts around line 20 in the generated wrapper
        // Adjust the line numbers to map back to the original user code
        const int wrapperOffset = 20;
        var adjustedLine = position.Line - wrapperOffset;
        
        return new Microsoft.CodeAnalysis.Text.LinePosition(adjustedLine, position.Character);
    }

    private LspDiagnosticSeverity ConvertSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity severity)
    {
        return severity switch
        {
            Microsoft.CodeAnalysis.DiagnosticSeverity.Error => LspDiagnosticSeverity.Error,
            Microsoft.CodeAnalysis.DiagnosticSeverity.Warning => LspDiagnosticSeverity.Warning,
            Microsoft.CodeAnalysis.DiagnosticSeverity.Info => LspDiagnosticSeverity.Information,
            Microsoft.CodeAnalysis.DiagnosticSeverity.Hidden => LspDiagnosticSeverity.Hint,
            _ => LspDiagnosticSeverity.Information
        };
    }

    private async Task<IEnumerable<LspDiagnostic>> ValidateCodeEngineRules(string code, Type? subjectType)
    {
        var diagnostics = new List<LspDiagnostic>();

        // Rule 1: Code length validation
        if (code.Length > 10000)
        {
            diagnostics.Add(new LspDiagnostic
            {
                Range = new LspRange(new Position(0, 0), new Position(0, code.Length)),
                Severity = LspDiagnosticSeverity.Error,
                Code = "CE001",
                Message = "Action code must not exceed 10,000 characters",
                Source = "Code Engine Rules"
            });
        }

        // Rule 2: Subject assignment validation
        if (code.Contains("Subject = "))
        {
            var lines = code.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Subject = "))
                {
                    var charIndex = lines[i].IndexOf("Subject = ");
                    diagnostics.Add(new LspDiagnostic
                    {
                        Range = new LspRange(new Position(i, charIndex), new Position(i, charIndex + 10)),
                        Severity = LspDiagnosticSeverity.Error,
                        Code = "CE002",
                        Message = "Direct assignment to Subject is not allowed. Modify Subject properties instead.",
                        Source = "Code Engine Rules"
                    });
                }
            }
        }

        // Rule 3: Validate subject property access if subject type is known
        if (subjectType != null)
        {
            await ValidateSubjectPropertyAccess(code, subjectType, diagnostics);
        }

        // Rule 4: Validate runtime value operations
        ValidateRuntimeValueOperations(code, diagnostics);

        return diagnostics;
    }

    private async Task ValidateSubjectPropertyAccess(string code, Type subjectType, List<LspDiagnostic> diagnostics)
    {
        var properties = subjectType.GetProperties().Select(p => p.Name).ToHashSet();
        var methods = subjectType.GetMethods().Where(m => m.IsPublic && !m.IsSpecialName).Select(m => m.Name).ToHashSet();

        var lines = code.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var subjectAccessIndex = line.IndexOf("Subject.");
            
            while (subjectAccessIndex >= 0)
            {
                var memberStart = subjectAccessIndex + 8; // "Subject.".Length
                var memberEnd = memberStart;
                
                // Find the end of the member name
                while (memberEnd < line.Length && (char.IsLetterOrDigit(line[memberEnd]) || line[memberEnd] == '_'))
                {
                    memberEnd++;
                }

                if (memberEnd > memberStart)
                {
                    var memberName = line.Substring(memberStart, memberEnd - memberStart);
                    
                    if (!properties.Contains(memberName) && !methods.Contains(memberName))
                    {
                        diagnostics.Add(new LspDiagnostic
                        {
                            Range = new LspRange(new Position(i, memberStart), new Position(i, memberEnd)),
                            Severity = LspDiagnosticSeverity.Error,
                            Code = "CE003",
                            Message = $"'{memberName}' is not a member of {subjectType.Name}",
                            Source = "Code Engine Rules"
                        });
                    }
                }

                // Look for next occurrence
                subjectAccessIndex = line.IndexOf("Subject.", memberEnd);
            }
        }
    }

    private void ValidateRuntimeValueOperations(string code, List<LspDiagnostic> diagnostics)
    {
        var lines = code.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            // Check for Read operations without proper error handling
            if (line.Contains("Read(") && !line.Contains("try") && !line.Contains("ContainsKey"))
            {
                var readIndex = line.IndexOf("Read(");
                diagnostics.Add(new LspDiagnostic
                {
                    Range = new LspRange(new Position(i, readIndex), new Position(i, readIndex + 4)),
                    Severity = LspDiagnosticSeverity.Warning,
                    Code = "CE004",
                    Message = "Consider checking if the key exists before reading runtime values to avoid exceptions",
                    Source = "Code Engine Rules"
                });
            }

            // Check for Save operations with empty keys
            if (line.Contains("Save(\"\")") || line.Contains("Save(\"\", "))
            {
                var saveIndex = line.IndexOf("Save(");
                diagnostics.Add(new LspDiagnostic
                {
                    Range = new LspRange(new Position(i, saveIndex), new Position(i, saveIndex + 4)),
                    Severity = LspDiagnosticSeverity.Warning,
                    Code = "CE005",
                    Message = "Runtime value key should not be empty",
                    Source = "Code Engine Rules"
                });
            }
        }
    }
}