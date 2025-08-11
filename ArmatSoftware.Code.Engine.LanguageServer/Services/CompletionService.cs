using Microsoft.CodeAnalysis;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ArmatSoftware.Code.Engine.LanguageServer.Services;
using LspSymbolKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.SymbolKind;
using RoslynSymbolKind = Microsoft.CodeAnalysis.SymbolKind;

namespace ArmatSoftware.Code.Engine.LanguageServer.Services;

public interface ICompletionService
{
    Task<CompletionList> GetCompletionsAsync(string code, int position, Type? subjectType = null);
}

public class CompletionService : ICompletionService
{
    private readonly ICodeEngineAnalysisService _analysisService;

    public CompletionService(ICodeEngineAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    public async Task<CompletionList> GetCompletionsAsync(string code, int position, Type? subjectType = null)
    {
        var symbols = await _analysisService.GetCompletionSymbolsAsync(code, position, subjectType);
        var completionItems = new List<CompletionItem>();

        foreach (var symbol in symbols)
        {
            var completionItem = CreateCompletionItem(symbol);
            if (completionItem != null)
            {
                completionItems.Add(completionItem);
            }
        }

        // Add Code Engine specific completions
        AddCodeEngineCompletions(completionItems, subjectType);

        return new CompletionList(completionItems, isIncomplete: false);
    }

    private CompletionItem? CreateCompletionItem(ISymbol symbol)
    {
        if (symbol == null) return null;

        var kind = GetCompletionItemKind(symbol);
        var detail = GetSymbolDetail(symbol);
        var documentation = GetSymbolDocumentation(symbol);

        return new CompletionItem
        {
            Label = symbol.Name,
            Kind = kind,
            Detail = detail,
            Documentation = documentation,
            InsertText = GetInsertText(symbol),
            FilterText = symbol.Name,
            SortText = GetSortText(symbol)
        };
    }

    private CompletionItemKind GetCompletionItemKind(ISymbol symbol)
    {
        return symbol.Kind switch
        {
            RoslynSymbolKind.Method => CompletionItemKind.Method,
            RoslynSymbolKind.Property => CompletionItemKind.Property,
            RoslynSymbolKind.Field => CompletionItemKind.Field,
            RoslynSymbolKind.Local => CompletionItemKind.Variable,
            RoslynSymbolKind.Parameter => CompletionItemKind.Variable,
            RoslynSymbolKind.NamedType => CompletionItemKind.Class,
            RoslynSymbolKind.Namespace => CompletionItemKind.Module,
            RoslynSymbolKind.Event => CompletionItemKind.Event,
            _ => CompletionItemKind.Text
        };
    }

    private string GetSymbolDetail(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol method => $"{method.ReturnType.Name} {method.Name}({string.Join(", ", method.Parameters.Select(p => $"{p.Type.Name} {p.Name}"))})",
            IPropertySymbol property => $"{property.Type.Name} {property.Name}",
            IFieldSymbol field => $"{field.Type.Name} {field.Name}",
            INamedTypeSymbol type => $"{type.TypeKind} {type.Name}",
            _ => symbol.ToDisplayString()
        };
    }

    private string GetSymbolDocumentation(ISymbol symbol)
    {
        var xmlDoc = symbol.GetDocumentationCommentXml();
        if (!string.IsNullOrEmpty(xmlDoc))
        {
            // Simple extraction of summary from XML documentation
            var summaryStart = xmlDoc.IndexOf("<summary>");
            var summaryEnd = xmlDoc.IndexOf("</summary>");
            if (summaryStart >= 0 && summaryEnd > summaryStart)
            {
                return xmlDoc.Substring(summaryStart + 9, summaryEnd - summaryStart - 9).Trim();
            }
        }
        return symbol.ToDisplayString();
    }

    private string GetInsertText(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol method when method.Parameters.Length > 0 => $"{method.Name}($0)",
            IMethodSymbol method => $"{method.Name}()",
            _ => symbol.Name
        };
    }

    private string GetSortText(ISymbol symbol)
    {
        // Prioritize certain symbols
        return symbol.Name switch
        {
            "Subject" => "0000_Subject",
            "Log" => "0001_Log",
            "Read" => "0002_Read",
            "Save" => "0003_Save",
            _ when symbol.ContainingNamespace?.Name == "System" => $"1000_{symbol.Name}",
            _ => $"2000_{symbol.Name}"
        };
    }

    private void AddCodeEngineCompletions(List<CompletionItem> completionItems, Type? subjectType)
    {
        // Add Code Engine specific completions
        var codeEngineItems = new[]
        {
            new CompletionItem
            {
                Label = "Subject",
                Kind = CompletionItemKind.Property,
                Detail = $"{subjectType?.Name ?? "object"} Subject",
                Documentation = "The subject object that this action operates on",
                InsertText = "Subject",
                SortText = "0000_Subject"
            },
            new CompletionItem
            {
                Label = "Log",
                Kind = CompletionItemKind.Property,
                Detail = "ILogger Log",
                Documentation = "Logger instance for writing log messages",
                InsertText = "Log",
                SortText = "0001_Log"
            },
            new CompletionItem
            {
                Label = "Read",
                Kind = CompletionItemKind.Method,
                Detail = "dynamic Read(string key)",
                Documentation = "Read a runtime value by key",
                InsertText = "Read(\"$0\")",
                SortText = "0002_Read"
            },
            new CompletionItem
            {
                Label = "Save",
                Kind = CompletionItemKind.Method,
                Detail = "void Save(string key, dynamic value)",
                Documentation = "Save a runtime value with the specified key",
                InsertText = "Save(\"$1\", $0)",
                SortText = "0003_Save"
            }
        };

        completionItems.AddRange(codeEngineItems);

        // Add subject-specific completions if subject type is known
        if (subjectType != null)
        {
            AddSubjectTypeCompletions(completionItems, subjectType);
        }
    }

    private void AddSubjectTypeCompletions(List<CompletionItem> completionItems, Type subjectType)
    {
        var properties = subjectType.GetProperties();
        foreach (var property in properties)
        {
            completionItems.Add(new CompletionItem
            {
                Label = $"Subject.{property.Name}",
                Kind = CompletionItemKind.Property,
                Detail = $"{property.PropertyType.Name} {property.Name}",
                Documentation = $"Property of {subjectType.Name}",
                InsertText = $"Subject.{property.Name}",
                SortText = $"0100_Subject_{property.Name}"
            });
        }

        var methods = subjectType.GetMethods().Where(m => m.IsPublic && !m.IsSpecialName);
        foreach (var method in methods)
        {
            var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            completionItems.Add(new CompletionItem
            {
                Label = $"Subject.{method.Name}",
                Kind = CompletionItemKind.Method,
                Detail = $"{method.ReturnType.Name} {method.Name}({parameters})",
                Documentation = $"Method of {subjectType.Name}",
                InsertText = method.GetParameters().Length > 0 ? $"Subject.{method.Name}($0)" : $"Subject.{method.Name}()",
                SortText = $"0101_Subject_{method.Name}"
            });
        }
    }
}