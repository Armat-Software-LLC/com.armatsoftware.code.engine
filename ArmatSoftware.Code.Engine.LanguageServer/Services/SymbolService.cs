using Microsoft.CodeAnalysis;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ArmatSoftware.Code.Engine.LanguageServer.Services;
using LspLocation = OmniSharp.Extensions.LanguageServer.Protocol.Models.Location;
using LspRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using LspSymbolKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.SymbolKind;

namespace ArmatSoftware.Code.Engine.LanguageServer.Services;

public interface ISymbolService
{
    Task<IEnumerable<DocumentSymbol>> GetDocumentSymbolsAsync(string code, Type? subjectType = null);
    Task<IEnumerable<LspLocation>> FindDefinitionsAsync(string code, int position, Type? subjectType = null);
    Task<Hover?> GetHoverInfoAsync(string code, int position, Type? subjectType = null);
}

public class SymbolService : ISymbolService
{
    private readonly ICodeEngineAnalysisService _analysisService;

    public SymbolService(ICodeEngineAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    public async Task<IEnumerable<DocumentSymbol>> GetDocumentSymbolsAsync(string code, Type? subjectType = null)
    {
        var symbols = new List<DocumentSymbol>();
        var semanticModel = await _analysisService.GetSemanticModelAsync(code, subjectType);
        var root = await semanticModel.SyntaxTree.GetRootAsync();

        // Find all variable declarations, method calls, etc.
        var walker = new SymbolWalker(semanticModel);
        walker.Visit(root);

        return walker.Symbols;
    }

    public async Task<IEnumerable<LspLocation>> FindDefinitionsAsync(string code, int position, Type? subjectType = null)
    {
        var semanticModel = await _analysisService.GetSemanticModelAsync(code, subjectType);
        var root = await semanticModel.SyntaxTree.GetRootAsync();
        var node = root.FindToken(position).Parent;

        if (node == null) return Enumerable.Empty<LspLocation>();

        var symbolInfo = semanticModel.GetSymbolInfo(node);
        var symbol = symbolInfo.Symbol;

        if (symbol == null) return Enumerable.Empty<LspLocation>();

        var locations = new List<LspLocation>();

        // Add definition locations
        foreach (var location in symbol.Locations)
        {
            if (location.IsInSource)
            {
                var lineSpan = location.GetLineSpan();
                locations.Add(new LspLocation
                {
                    Uri = location.SourceTree?.FilePath ?? "file:///temp.cs",
                    Range = new LspRange
                    {
                        Start = new Position(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character),
                        End = new Position(lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character)
                    }
                });
            }
        }

        return locations;
    }

    public async Task<Hover?> GetHoverInfoAsync(string code, int position, Type? subjectType = null)
    {
        var semanticModel = await _analysisService.GetSemanticModelAsync(code, subjectType);
        var root = await semanticModel.SyntaxTree.GetRootAsync();
        var node = root.FindToken(position).Parent;

        if (node == null) return null;

        var symbolInfo = semanticModel.GetSymbolInfo(node);
        var symbol = symbolInfo.Symbol;

        if (symbol == null) return null;

        var hoverContent = CreateHoverContent(symbol, subjectType);
        if (hoverContent == null) return null;

        var location = node.GetLocation();
        var lineSpan = location.GetLineSpan();

        return new Hover
        {
            Contents = new MarkedStringsOrMarkupContent(hoverContent),
            Range = new LspRange
            {
                Start = new Position(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character),
                End = new Position(lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character)
            }
        };
    }

    private MarkupContent? CreateHoverContent(ISymbol symbol, Type? subjectType)
    {
        var content = new List<string>();

        // Add symbol signature
        content.Add($"```csharp\n{GetSymbolSignature(symbol)}\n```");

        // Add documentation
        var documentation = GetSymbolDocumentation(symbol, subjectType);
        if (!string.IsNullOrEmpty(documentation))
        {
            content.Add(documentation);
        }

        // Add Code Engine specific information
        var codeEngineInfo = GetCodeEngineSpecificInfo(symbol, subjectType);
        if (!string.IsNullOrEmpty(codeEngineInfo))
        {
            content.Add(codeEngineInfo);
        }

        if (content.Count == 0) return null;

        return new MarkupContent
        {
            Kind = MarkupKind.Markdown,
            Value = string.Join("\n\n", content)
        };
    }

    private string GetSymbolSignature(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol method => $"{method.ReturnType.ToDisplayString()} {method.Name}({string.Join(", ", method.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"))})",
            IPropertySymbol property => $"{property.Type.ToDisplayString()} {property.Name} {{ {(property.GetMethod != null ? "get; " : "")}{(property.SetMethod != null ? "set; " : "")}}}",
            IFieldSymbol field => $"{field.Type.ToDisplayString()} {field.Name}",
            ILocalSymbol local => $"{local.Type.ToDisplayString()} {local.Name}",
            IParameterSymbol parameter => $"{parameter.Type.ToDisplayString()} {parameter.Name}",
            INamedTypeSymbol type => $"{type.TypeKind.ToString().ToLower()} {type.ToDisplayString()}",
            _ => symbol.ToDisplayString()
        };
    }

    private string GetSymbolDocumentation(ISymbol symbol, Type? subjectType)
    {
        // Check for XML documentation
        var xmlDoc = symbol.GetDocumentationCommentXml();
        if (!string.IsNullOrEmpty(xmlDoc))
        {
            var summaryStart = xmlDoc.IndexOf("<summary>");
            var summaryEnd = xmlDoc.IndexOf("</summary>");
            if (summaryStart >= 0 && summaryEnd > summaryStart)
            {
                var summary = xmlDoc.Substring(summaryStart + 9, summaryEnd - summaryStart - 9).Trim();
                return summary;
            }
        }

        // Provide default documentation for common symbols
        return symbol.Name switch
        {
            "Subject" => $"The {subjectType?.Name ?? "subject"} object that this action operates on.",
            "Log" => "Logger instance for writing log messages during action execution.",
            "Read" => "Reads a runtime value that was previously saved with the Save method.",
            "Save" => "Saves a runtime value that can be accessed later in the same execution or by other actions.",
            _ => null
        };
    }

    private string GetCodeEngineSpecificInfo(ISymbol symbol, Type? subjectType)
    {
        return symbol.Name switch
        {
            "Subject" when subjectType != null => $"**Type:** {subjectType.Name}\n\n**Available Properties:**\n{string.Join("\n", subjectType.GetProperties().Select(p => $"- `{p.Name}`: {p.PropertyType.Name}"))}",
            "Read" => "**Usage:** `Read(\"keyName\")` - Returns the value associated with the key, or throws an exception if the key doesn't exist.",
            "Save" => "**Usage:** `Save(\"keyName\", value)` - Stores a value that can be retrieved later using Read().",
            "Log" => "**Available Methods:**\n- `LogInformation(message)`\n- `LogWarning(message)`\n- `LogError(message)`\n- `LogDebug(message)`",
            _ => null
        };
    }
}

// Helper class to walk the syntax tree and collect symbols
internal class SymbolWalker : Microsoft.CodeAnalysis.CSharp.CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    public List<DocumentSymbol> Symbols { get; } = new();

    public SymbolWalker(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public override void VisitVariableDeclarator(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            var location = node.GetLocation();
            var lineSpan = location.GetLineSpan();
            
            Symbols.Add(new DocumentSymbol
            {
                Name = symbol.Name,
                Kind = LspSymbolKind.Variable,
                Range = new LspRange
                {
                    Start = new Position(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character),
                    End = new Position(lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character)
                },
                SelectionRange = new LspRange
                {
                    Start = new Position(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character),
                    End = new Position(lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character)
                }
            });
        }

        base.VisitVariableDeclarator(node);
    }

    public override void VisitInvocationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax node)
    {
        var symbolInfo = _semanticModel.GetSymbolInfo(node);
        var symbol = symbolInfo.Symbol;
        
        if (symbol != null)
        {
            var location = node.GetLocation();
            var lineSpan = location.GetLineSpan();
            
            Symbols.Add(new DocumentSymbol
            {
                Name = symbol.Name,
                Kind = LspSymbolKind.Function,
                Range = new LspRange
                {
                    Start = new Position(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character),
                    End = new Position(lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character)
                },
                SelectionRange = new LspRange
                {
                    Start = new Position(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character),
                    End = new Position(lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character)
                }
            });
        }

        base.VisitInvocationExpression(node);
    }
}