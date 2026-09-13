# Code Engine

Code Engine lets applications run business logic that can be composed and maintained outside the main compiled codebase. It gives business users and developers room to adapt application behavior while keeping execution controlled, testable, observable, and fast.

It fits naturally into ASP.NET Core applications through dependency injection and can be added to both new and existing solutions.

## Why Code Engine?

Use Code Engine when parts of your application logic need to change more often than the application itself. A typical integration looks like this:

1. Add the Code Engine NuGet package.
2. Register the engine and any storage adapters you need.
3. Inject an executor wherever custom logic should run.
4. Store, test, and update actions without rebuilding the host application.

Code Engine is designed for high-throughput environments and aims to deliver performance close to compiled code, without giving up the flexibility of dynamic logic.

## Terminology

**Subject** is the object processed by custom logic. It is passed to and returned from `IExecutor<T>.Execute()`. The host application defines the subject type and its public properties.

**Subject Action** (`ISubjectAction`) contains the language and source code for custom logic. Actions are stored through an `IActionRepository` implementation and retrieved through an `IActionProvider` implementation.

**Executor** (`IExecutor<T>`) is the dynamically generated implementation that runs custom logic against a subject.

**Executor Catalog** (`IExecutorCatalog<T>`) looks up an executor for a subject type by key. Use a catalog when an application has multiple executors for the same subject type.

## Get Started

1. Add the main package to your solution:

   ```bash
   dotnet add package com.armatsoftware.code.engine
   ```

2. Add optional packages when needed, for example:

   ```bash
   dotnet add package com.armatsoftware.code.engine.storage
   dotnet add package com.armatsoftware.code.engine.storage.file
   ```

3. Register the services you plan to use:

   ```csharp
   services.UseCodeEngine(new CodeEngineOptions
   {
       CacheExpirationMinutes = 1,
       CodeEngineNamespace = "codeengineuniquenamespace",
       CompilerType = CompilerTypeEnum.Vb,
       Logger = new CustomLogger()
   });

   services.UseCodeEngineStorage();

   services.UseCodeEngineFileAdapter(new FileStorageOptions
   {
       FileExtension = "code",
       StoragePath = "/tmp/demo/"
   });
   ```

   Create and activate a subject action before executing it. `IActionStorage.AddAction<TSubject>()` creates the action, adds its first revision, and activates that revision:

   ```csharp
   var actionStorage = serviceProvider.GetRequiredService<IActionStorage>();

   actionStorage.AddAction<SubjectModel>(
       name: "SetMessage",
       code: "Subject.Message = \"Hello from Code Engine\"",
       author: "demo",
       comment: "Initial message action");
   ```

4. Inject an executor and run it where you need custom behavior:

   ```csharp
   public class SimpleService
   {
       private readonly IExecutorCatalog<SubjectModel> _messageGenerators;
       private readonly IExecutor<SubjectModel> _defaultGenerator;

       public SimpleService(
           IExecutorCatalog<SubjectModel> messageGenerators,
           IExecutor<SubjectModel> defaultGenerator)
       {
           _messageGenerators = messageGenerators;
           _defaultGenerator = defaultGenerator;
       }

       public SubjectModel SaySomething(string? about)
       {
           if (string.IsNullOrWhiteSpace(about))
           {
               return _defaultGenerator.Execute(new SubjectModel());
           }

           var executor = _messageGenerators.ForKey(about);
           return executor.Execute(new SubjectModel());
       }
   }
   ```

## OpenTelemetry

Code Engine emits a `codeengine.execute` span for each custom-code execution through the `ArmatSoftware.Code.Engine` activity source. Register that source with the OpenTelemetry provider in the host application:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("ArmatSoftware.Code.Engine"));
```

Execution spans include the subject type, executor key when available, compiler type, and action count. Exceptions are recorded and mark the span as failed.

Code Engine does not configure exporters or add an OpenTelemetry SDK dependency. The host application remains responsible for choosing and configuring its exporters.

### Docker and Aspire Dashboard Demo

The tester API can run beside the standalone Aspire Dashboard to demonstrate request and Code Engine execution spans:

```bash
docker compose up --build
```

Open `http://localhost:18888` for the Aspire Dashboard, then call `http://localhost:8080/api/CodeEngine/execute_default`. The API sends its ASP.NET Core request span and nested Code Engine execution span to the dashboard over OTLP/gRPC.

Stop the demo with:

```bash
docker compose down
```

## Version History

- **1.x.x**: Added the core contracts and base implementation for injection, compilation, and execution, along with initialization, file storage, and file logging.
- **2.x.x**: Added keyed executor lookup and refactored file storage.
- **3.x.x**: Improved initialization and added logging from custom code.
- **4.x.x (current)**: Refactored the storage abstraction and improved the default storage-management and file-adapter implementations.

## What's New In 4.x

Custom code can write information through the executor's `Log` property. The logger supports three message categories: `Info`, `Warning`, and `Error`. Messages are handled by the logger supplied during Code Engine initialization.

The `IExecutor<T>` API is also simpler: `Execute()` accepts a subject and returns that same subject with the changes applied by custom code. When an executor catalog is available, an executor can be selected by key before execution.

```csharp
// Directly inject an executor for a subject type.
public IActionResult SayHello([FromServices] IExecutor<MessageModel> executor)
{
    return View(executor.Execute(new MessageModel()));
}

// Look up an executor by key through the catalog.
public IActionResult SayHelloWithKey(
    string key,
    [FromServices] IExecutorCatalog<MessageModel> catalog)
{
    var executor = catalog.ForKey(key);
    return View(executor.Execute(new MessageModel()));
}
```

## Roadmap

The next major usability improvement is a faster way to compose actions. A simple web-based interface for creating and managing custom logic is being considered.

## Links

[Project website](https://armatsoftware.com/code-engine/)
