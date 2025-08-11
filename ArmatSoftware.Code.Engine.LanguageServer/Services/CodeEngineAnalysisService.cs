using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ArmatSoftware.Code.Engine.Core;
using System.Reflection;

namespace ArmatSoftware.Code.Engine.LanguageServer.Services;

public interface ICodeEngineAnalysisService
{
    Task<SemanticModel> GetSemanticModelAsync(string code, Type? subjectType = null);
    Task<IEnumerable<ISymbol>> GetAvailableSymbolsAsync(Type? subjectType = null);
    Task<Compilation> CreateCompilationAsync(string code, Type? subjectType = null);
    Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(string code, Type? subjectType = null);
    Task<IEnumerable<ISymbol>> GetCompletionSymbolsAsync(string code, int position, Type? subjectType = null);
}

public class CodeEngineAnalysisService : ICodeEngineAnalysisService
{
    private readonly Dictionary<string, MetadataReference> _references;
    private readonly CSharpCompilationOptions _compilationOptions;

    public CodeEngineAnalysisService()
    {
        _references = new Dictionary<string, MetadataReference>();
        _compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        // Add core .NET references
        var coreAssemblies = new[]
        {
            typeof(object).Assembly, // System.Private.CoreLib
            typeof(Console).Assembly, // System.Console
            typeof(IEnumerable<>).Assembly, // System.Collections
            typeof(System.Linq.Enumerable).Assembly, // System.Linq
            Assembly.Load("System.Runtime"),
            Assembly.Load("netstandard"),
            Assembly.Load("Microsoft.CSharp")
        };

        foreach (var assembly in coreAssemblies)
        {
            if (!string.IsNullOrEmpty(assembly.Location))
            {
                _references[assembly.GetName().Name!] = MetadataReference.CreateFromFile(assembly.Location);
            }
        }

        // Add Code Engine references
        var codeEngineAssemblies = new[]
        {
            typeof(IExecutor<>).Assembly // Core
        };

        foreach (var assembly in codeEngineAssemblies)
        {
            if (!string.IsNullOrEmpty(assembly.Location))
            {
                _references[assembly.GetName().Name!] = MetadataReference.CreateFromFile(assembly.Location);
            }
        }
    }

    public async Task<SemanticModel> GetSemanticModelAsync(string code, Type? subjectType = null)
    {
        var compilation = await CreateCompilationAsync(code, subjectType);
        var syntaxTree = compilation.SyntaxTrees.First();
        return compilation.GetSemanticModel(syntaxTree);
    }

    public async Task<IEnumerable<ISymbol>> GetAvailableSymbolsAsync(Type? subjectType = null)
    {
        var symbols = new List<ISymbol>();
        
        // Create a minimal compilation to get global symbols
        var emptyCode = GenerateContextCode("", subjectType);
        var compilation = await CreateCompilationAsync(emptyCode, subjectType);
        
        // Get global namespace symbols
        var globalNamespace = compilation.GlobalNamespace;
        CollectSymbols(globalNamespace, symbols);
        
        return symbols;
    }

    public async Task<Compilation> CreateCompilationAsync(string code, Type? subjectType = null)
    {
        var fullCode = GenerateContextCode(code, subjectType);
        var syntaxTree = CSharpSyntaxTree.ParseText(fullCode);
        
        var references = _references.Values.ToList();
        
        // Add subject type assembly reference if provided
        if (subjectType != null && !string.IsNullOrEmpty(subjectType.Assembly.Location))
        {
            var subjectRef = MetadataReference.CreateFromFile(subjectType.Assembly.Location);
            references.Add(subjectRef);
        }

        var compilation = CSharpCompilation.Create(
            "CodeEngineAnalysis",
            new[] { syntaxTree },
            references,
            _compilationOptions);

        return await Task.FromResult(compilation);
    }

    public async Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(string code, Type? subjectType = null)
    {
        var compilation = await CreateCompilationAsync(code, subjectType);
        var diagnostics = compilation.GetDiagnostics();
        
        // Filter out diagnostics that are not relevant to user code
        return diagnostics.Where(d => 
            d.Severity == DiagnosticSeverity.Error || 
            d.Severity == DiagnosticSeverity.Warning)
            .Where(d => !IsGeneratedCodeDiagnostic(d));
    }

    public async Task<IEnumerable<ISymbol>> GetCompletionSymbolsAsync(string code, int position, Type? subjectType = null)
    {
        var semanticModel = await GetSemanticModelAsync(code, subjectType);
        var syntaxTree = semanticModel.SyntaxTree;
        var root = await syntaxTree.GetRootAsync();
        
        // Find the node at the given position
        var node = root.FindToken(position).Parent;
        var symbols = new List<ISymbol>();
        
        // Get symbols in scope at the current position
        var symbolInfo = semanticModel.LookupSymbols(position);
        symbols.AddRange(symbolInfo);
        
        // Add namespace symbols
        var namespaceSymbols = semanticModel.LookupNamespacesAndTypes(position);
        symbols.AddRange(namespaceSymbols);
        
        return symbols.Distinct();
    }

    private string GenerateContextCode(string userCode, Type? subjectType)
    {
        var subjectTypeName = subjectType?.Name ?? "object";
        var subjectNamespace = subjectType?.Namespace ?? "System";
        
        return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmatSoftware.Code.Engine.Core;
using {subjectNamespace};
using Microsoft.Extensions.Logging;

namespace CodeEngineUserCode
{{
    public class UserAction : IExecutionContext
    {{
        public {subjectTypeName} Subject {{ get; set; }}
        public ILogger Log {{ get; set; }}
        
        private Dictionary<string, dynamic> _runtimeValues = new Dictionary<string, dynamic>();
        
        public dynamic Read(string key)
        {{
            return _runtimeValues[key];
        }}
        
        public void Save(string key, dynamic value)
        {{
            _runtimeValues[key] = value;
        }}
        
        public void Execute()
        {{
            {userCode}
        }}
    }}
}}";
    }

    private void CollectSymbols(INamespaceSymbol namespaceSymbol, List<ISymbol> symbols)
    {
        symbols.AddRange(namespaceSymbol.GetTypeMembers());
        
        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            symbols.Add(childNamespace);
            CollectSymbols(childNamespace, symbols);
        }
    }

    private bool IsGeneratedCodeDiagnostic(Diagnostic diagnostic)
    {
        // Filter out diagnostics from generated wrapper code
        var location = diagnostic.Location;
        if (location.IsInSource)
        {
            var lineSpan = location.GetLineSpan();
            // User code typically starts after the generated wrapper
            return lineSpan.StartLinePosition.Line < 20;
        }
        return false;
    }
}