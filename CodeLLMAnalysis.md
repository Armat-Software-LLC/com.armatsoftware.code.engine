# Code Engine Solution Analysis

**Code Engine** is a sophisticated .NET framework designed to enable dynamic code execution within applications. It allows business users to maintain and execute custom logic fragments in a secure, controlled environment without requiring application redeployment.

## Core Architecture

The solution follows a modular architecture with clear separation of concerns:

### 1. **Core Components** (`ArmatSoftware.Code.Engine.Core`)
- **IExecutor<TSubject>**: The main execution interface that processes subjects through actions
- **IExecutableAction**: Defines actions with name, code, and execution order
- **ISubjectAction<TSubject>**: Actions specific to subject types
- **IExecutionContext**: Provides runtime context for action execution

### 2. **Compilation System** (`ArmatSoftware.Code.Engine.Compiler`)
- **ICompiler<TSubject>**: Interface for code compilation
- **CSharpCompiler**: Uses Roslyn to compile C# code dynamically at runtime
- **Template Generation**: Uses T4 templates to generate executor classes
- **Dependency Injection**: Comprehensive DI setup for framework components

### 3. **Storage Layer** (`ArmatSoftware.Code.Engine.Storage`)
- **Versioning System**: Supports multiple revisions of actions with activation control
- **Data Integrity**: Ensures completeness and consistency of stored actions
- **Storage Adapters**: Pluggable storage implementations (file-based included)
- **Action Management**: CRUD operations for actions with revision control

### 4. **Execution Engine**
- **ExecutorFactory**: Creates and manages executor instances
- **ExecutorCache**: Provides caching for compiled executors
- **ExecutorCatalog**: Manages collections of executors by key
- **Thread Safety**: Supports concurrent execution through executor cloning

### 5. **Language Server** (`ArmatSoftware.Code.Engine.LanguageServer`) ⭐ **NEW**
- **LSP Implementation**: Full Language Server Protocol support for external editors
- **IntelliSense**: Code completion for Code Engine actions and subject types
- **Design-Time Diagnostics**: Real-time error checking and validation
- **Symbol Services**: Hover information, go-to-definition, document outline
- **Editor Integration**: Sample VS Code extension and configuration

## Key Features

### **Dynamic Code Compilation**
- Uses Microsoft.CodeAnalysis (Roslyn) for runtime C# compilation
- Generates executor classes from T4 templates
- Automatic reference resolution for required assemblies
- Comprehensive error handling and validation

### **Subject-Action Pattern**
- **Subjects**: Domain objects that actions operate on (e.g., `StringOnlySubject`)
- **Actions**: Code fragments that modify or process subjects
- **Execution Order**: Actions execute in specified sequence
- **Runtime Context**: Actions can save/read values during execution

### **Storage & Versioning**
- **Revision Control**: Each action update creates a new revision
- **Activation System**: Only activated revisions are used in execution
- **Data Protection**: Prevents corruption through validation and integrity checks
- **Flexible Storage**: Supports different storage adapters (file system, database, etc.)

### **Security & Performance**
- **Code Length Limits**: Actions limited to 10,000 characters
- **Compilation Validation**: Comprehensive error checking before execution
- **Memory Management**: Efficient assembly loading and disposal
- **Caching**: Compiled executors are cached for performance

### **Developer Experience** ⭐ **NEW**
- **Language Server**: LSP-compliant server for editor integration
- **IntelliSense**: Auto-completion for Code Engine API and subject properties
- **Real-time Validation**: Design-time error checking with custom rules
- **Multi-Editor Support**: Works with VS Code, Vim, Emacs, Sublime Text, etc.

## Usage Patterns

### **Basic Execution**
```csharp
// Simple execution
executor.Execute(new StringOnlySubject());

// Keyed execution
var executor = catalog.ForKey("specific-key");
executor.Execute(subject);
```

### **Action Management**
```csharp
// Create new action
repo.AddAction<StringOnlySubject>("ActionName", "Subject.Data = \"Hello\";", "Author", "Comment", "Key");

// Update existing action (creates new revision)
repo.UpdateAction<StringOnlySubject>("ActionName", "Subject.Data = \"Updated\";", "Author", "Comment", "Key");

// Activate specific revision
repo.ActivateRevision<StringOnlySubject>("ActionName", revisionId, "Key");
```

### **Configuration & Setup**
```csharp
// Basic setup
builder.Services.UseCodeEngine(new CodeEngineOptions());

// With storage
builder.Services.UseCodeEngineStorage();

// With custom storage adapter
builder.Services.UseCodeEngineStorage(new CustomStorageAdapter());
```

### **Language Server Integration** ⭐ **NEW**
```bash
# Start the language server
dotnet run --project ArmatSoftware.Code.Engine.LanguageServer

# Configure in VS Code extension
{
  "contributes": {
    "languages": [{
      "id": "codeengine",
      "extensions": [".ce"]
    }]
  }
}
```

## Project Structure

### **Main Projects**
- **ArmatSoftware.Code.Engine.Core**: Core interfaces and abstractions
- **ArmatSoftware.Code.Engine.Compiler**: Compilation engine with C# support
- **ArmatSoftware.Code.Engine.Storage**: Storage layer with versioning
- **ArmatSoftware.Code.Engine.Storage.File**: File-based storage adapter
- **ArmatSoftware.Code.Engine.LanguageServer**: LSP server for editor integration ⭐ **NEW**

### **Supporting Projects**
- **ArmatSoftware.Code.Engine.Tester.WebApi**: Web API for testing and demonstration
- **ArmatSoftware.Code.Engine.Tests.Unit**: Comprehensive unit tests
- **ArmatSoftware.Code.Engine.Storage.Tests**: Storage layer tests
- **ArmatSoftware.Code.Engine.Storage.File.Tests**: File adapter tests

## Benefits

1. **Business Agility**: Non-developers can modify business logic without deployments
2. **Version Control**: Safe updates with rollback capabilities
3. **Performance**: Compiled code execution (not interpreted)
4. **Security**: Controlled execution environment with validation
5. **Scalability**: Thread-safe execution with caching
6. **Extensibility**: Pluggable storage and compiler implementations
7. **Developer Experience**: Rich IDE support with IntelliSense and error checking ⭐ **NEW**
8. **Editor Agnostic**: Works with any LSP-compatible editor ⭐ **NEW**

## Use Cases

- **Business Rules Engine**: Dynamic business logic that changes frequently
- **Configuration-Driven Applications**: Applications where behavior is configured rather than coded
- **Multi-Tenant Systems**: Different logic per tenant without separate deployments
- **A/B Testing**: Different logic versions for testing
- **Workflow Systems**: Dynamic step definitions and processing logic
- **Low-Code Platforms**: Enable business users to create custom logic with IDE support ⭐ **NEW**

## Technical Implementation Details

### **Code Generation Process**
1. Actions are collected and validated
2. T4 template generates C# executor class
3. Roslyn compiles the generated code
4. Assembly is loaded and executor instance created
5. Executor is cached for subsequent use

### **Execution Flow**
1. Subject is passed to executor
2. Actions execute in order
3. Each action can modify the subject
4. Runtime values can be saved/retrieved between actions
5. Final subject state is returned

### **Storage Architecture**
- Actions are stored with metadata (author, comments, timestamps)
- Each update creates a new revision
- Only activated revisions are used for execution
- Storage adapters provide flexibility for different backends

### **Language Server Architecture** ⭐ **NEW**
- **Semantic Analysis**: Uses Roslyn for C# code analysis
- **Context Generation**: Wraps user code in proper execution context
- **Diagnostics**: Custom validation rules for Code Engine patterns
- **Completions**: IntelliSense for framework API and subject types
- **Protocol Compliance**: Full LSP specification implementation

## Development Workflow Enhancement ⭐ **NEW**

### **Before Language Server**
- Write code in basic text editors
- No IntelliSense or error checking
- Runtime discovery of issues
- Manual documentation lookup

### **After Language Server**
- Rich IDE experience in any editor
- Real-time error detection
- Auto-completion for all APIs
- Hover documentation
- Go-to-definition support
- Design-time validation

The solution demonstrates enterprise-grade architecture with comprehensive testing, proper dependency injection, and production-ready features like caching, logging, error handling, and now rich developer tooling support.