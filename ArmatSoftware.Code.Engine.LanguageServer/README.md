# Code Engine Language Server

## Overview

The Code Engine Language Server provides Language Server Protocol (LSP) support for external code editors when working with Code Engine actions. It enables IntelliSense, design-time error checking, hover information, and other IDE features for Code Engine action development.

## Features

### IntelliSense & Code Completion
- **Subject Properties**: Auto-completion for subject type properties and methods
- **Code Engine API**: Completion for `Subject`, `Log`, `Read()`, `Save()` methods
- **Standard Libraries**: Full C# standard library support
- **Context-Aware**: Completions based on current cursor position and scope

### Design-Time Diagnostics
- **Compilation Errors**: Real-time C# compilation error detection
- **Code Engine Rules**: 
  - Code length validation (max 10,000 characters)
  - Subject assignment prevention
  - Runtime value operation validation
  - Subject property access validation

### Symbol Information
- **Hover Information**: Detailed symbol information with documentation
- **Go to Definition**: Navigate to symbol definitions
- **Document Symbols**: Outline view of variables and method calls

### Supported File Types
- `.ce` files (Code Engine action files)
- `.cs` files (C# files in Code Engine context)

## Architecture

### Core Services
- **CodeEngineAnalysisService**: Provides semantic analysis using Roslyn
- **CompletionService**: Generates IntelliSense completions
- **DiagnosticsService**: Validates code and provides error/warning information
- **SymbolService**: Handles symbol resolution and navigation

### LSP Handlers
- **TextDocumentSyncHandler**: Manages document lifecycle and publishes diagnostics
- **CompletionHandler**: Provides code completion
- **HoverHandler**: Shows symbol information on hover
- **DefinitionHandler**: Enables go-to-definition functionality
- **DocumentSymbolHandler**: Provides document outline

## Usage

### Starting the Language Server
```bash
dotnet run --project ArmatSoftware.Code.Engine.LanguageServer
```

The server communicates via stdin/stdout using the Language Server Protocol.

### Editor Integration

#### Visual Studio Code
Create a VS Code extension that launches the language server:

```json
{
  "contributes": {
    "languages": [{
      "id": "codeengine",
      "aliases": ["Code Engine", "codeengine"],
      "extensions": [".ce"],
      "configuration": "./language-configuration.json"
    }],
    "grammars": [{
      "language": "codeengine",
      "scopeName": "source.csharp",
      "path": "./syntaxes/csharp.tmGrammar.json"
    }]
  },
  "activationEvents": [
    "onLanguage:codeengine"
  ],
  "main": "./out/extension.js"
}
```

#### Other Editors
The language server follows the LSP specification and can be integrated with any LSP-compatible editor:
- **Vim/Neovim**: Using coc.nvim or built-in LSP
- **Emacs**: Using lsp-mode
- **Sublime Text**: Using LSP package
- **Atom**: Using atom-languageclient

### Configuration

The language server can be configured via `appsettings.json`:

```json
{
  "subjectTypes": {
    "YourSubjectType": {
      "namespace": "Your.Namespace",
      "properties": [
        {
          "name": "PropertyName",
          "type": "PropertyType",
          "description": "Property description"
        }
      ]
    }
  }
}
```

## Code Engine Context

### Available Symbols
When writing Code Engine actions, the following symbols are available:

- **Subject**: The subject object being processed
- **Log**: Logger instance for writing messages
- **Read(string key)**: Read runtime values
- **Save(string key, dynamic value)**: Save runtime values

### Example Action
```csharp
// Set subject data
Subject.Data = "Hello, World!";

// Log information
Log.LogInformation("Processing subject");

// Save runtime value
Save("processedAt", DateTime.Now);

// Read runtime value (in subsequent actions)
var timestamp = Read("processedAt");
```

### Validation Rules
- Maximum code length: 10,000 characters
- Direct Subject assignment is not allowed
- Runtime value keys should not be empty
- Subject property access is validated against the actual subject type

## Development

### Building
```bash
dotnet build ArmatSoftware.Code.Engine.LanguageServer
```

### Testing
The language server can be tested using LSP client tools or by integrating with an editor.

### Extending
To add support for new subject types:
1. Update `appsettings.json` with the new type definition
2. The analysis service will automatically provide IntelliSense for the new type

## Dependencies

- **OmniSharp.Extensions.LanguageServer**: LSP implementation
- **Microsoft.CodeAnalysis**: Roslyn for C# analysis
- **ArmatSoftware.Code.Engine.Core**: Core Code Engine interfaces
- **ArmatSoftware.Code.Engine.Compiler**: Compilation services

## Troubleshooting

### Common Issues
1. **No IntelliSense**: Ensure the language server is running and the editor is configured correctly
2. **Incorrect Completions**: Check subject type configuration in `appsettings.json`
3. **Compilation Errors**: Verify that all required references are available

### Logging
The language server logs to console. Set log level to Debug for detailed information:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```