# Table of Contents
1. [Code Engine](#code-engine)
2. [Why Code Engine](#why-code-engine)
3. [Terminology](#terminology)
4. [How to use it?](#how-to-use-it)
5. [Versions](#versions)
6. [What's in this version?](#whats-in-this-version)
7. [What's next?](#whats-next)
8. [Links](#links)

# Code Engine

Code Engine is a simple, yet powerful, code execution engine. It is designed to be used in applications, where you want to empower your business users to maintain fragments of the application logic in a secure and controlled environment, offer the level of flexibility and maintainability that is not possible with the compiled and deployed code, and to do so without sacrificing the performance and security of the application.

# Why Code Engine?

Code Engine allows composition, testing,and integration of custom code execution in your web application. It is designed to be secure and easy to use. It is also designed to be easily extendable, so you can add your own implementations to it.

For brand new products, adding Code Engine is as easy as adding the NuGet package to your solution, adding some initialization code, injecting specific "executors" throughout your application, and either providing hard-coded logic for their implementation or handing the responsibility to maintain that custom logic to your power users.

For existing products, you can use Code Engine to replace some of the existing logic with a more flexible and maintainable solution. It is easy to incorporate into an existing application because it works with the existing dependency injection framework in ASP.NET Core.

Code Engine performance is as close to that of the compiled and deployed code as it gets. It is designed to be fast and efficient, to be used in environments with heavy load and high performance demands.

# Terminology

**Subject** - the object that is being processed by the custom logic. It is the object that is being passed to and returned from the `Execute()` method of the `IExecutor<T>`. This is an arbitrary type, defined and used by the host application. This object can either be the target of the intended updates or contain other public properties that are target of the custom logic. It can be as shallow or as complex as the host application developer needs it to be.

**Subject Action** (`ISubjectAction`) - a special object that contains information and instruction in form of one of the available language syntaxes (C# or VB.NET) that represents the custom logic to be executed against **Subject**. It is the object that is being stored and maintained via an implementation of `IActionRepository` and used via an implementation of `IActionProvider`.

**Executor** (`IExecutor<T>`) - the contract whose dynamically generated implementation is injected into the host application and used to execute the custom logic against the **Subject**.

**Executor Catalog** (`IExecutorCatalog<T>`) - the contract injected into the host application and used to look up a specific executor for a **Subject** type using a key. This is useful when there are multiple executors for the same **Subject** type and the host application needs to pick the right one based on some criteria.

# How to use it?

1. Start with an existing solution or brand new.
2. Look up NuGet packages for "armatsoftware.code.engine".
3. Add the main package `com.armatsoftware.code.engine` to your solution.
4. Add supplemental packages, if needed. Ex: `com.armatsoftware.code.engine.storage.file`.
5. Initialize Code Engine. Ex:
    ``` c#
    builder.Services.UseCodeEngine(new CodeEngineOptions()
    {
        CodeEngineNamespace = "some.unique.namespace",
        CompilerType = CompilerTypeEnum.CSharp,
        Logger = new ConsoleLogger(),
        Provider = new SimpleActionProvider()
    });
   ```
6. Inject and use `IExecutor<T>` as needed. Ex:
    ``` c#
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

# Versions
- 1.x.x - essential contracts and base implementation for injection, compilation and execution of the custom logic, including initialization method and implementations for the file storage and file logger
- 2.x.x - added keyed executor lookup and rafactored file storage
- 3.x.x (current) - refactored and improved initialization and ability to log from custom code

# What's in this version?

The major update of this version is the ability to log information from within custom code. This is done by using available within executors `Log {get;}' property offering three categories of messages to log (Info, Warning, Error). The messages are logged using the logger provided during the initialization of the Code Engine.

Another major update is the simplified interface of the `IExecutor<T>`. Now, the `Execute()` method is able to take in the subject model object and returns the same object, updated according to custom code. This allows for a more natural and fluent use of the executor. Method `Execute()` is also able to take in the key of the executor to be used, if the executor catalog is available. is no longer available, as it is no longer needed. Property `Subject` is now read-only and reflects the updates applied by `Execute()`.

``` c#
    /// <summary>
    /// Direct injection of the executor for subject type MessageModel
    /// </summary>
    /// <param name="executor"></param>
    /// <returns></returns>
    public IActionResult SayHello([FromServices] IExecutor<MessageModel> executor)
    {
        return View(new MessageModel() { Message = executor.Execute(new MessageModel()).Message });
    }
    
    /// <summary>
    /// Injection of the executor catalog for subject type MessageModel.
    /// Lookup by key.
    /// </summary>
    /// <param name="key">Specific key to look up necessary IExecutor type</param>
    /// <param name="catalog">Catalog of IExecutor options to pick from using key</param>
    /// <returns></returns>
    public IActionResult SayHelloWithKey(string key, [FromServices] IExecutorCatalog<MessageModel> catalog)
    {
        var executor = catalog.ForKey(key);
        return View(new MessageModel() { Message = executor.Execute(new MessageModel()).Message });
    }
```

# What's next?

- I realize that the next big update should be within the usability area and the best starting point there is to offer a way to quickly compose the actions. I am looking at options of creating a simple web-based UI to allow for the composition of the custom logic.


# Links

[Project Web Site](https://armatsoftware.com/code-engine/)