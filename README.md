# Table of Contents
1. [Code Engine](#code-engine)
2. [Why Code Engine](#why-code-engine)
3. [How to use it?](#how-to-use-it)
4. [Versions](#versions)
5. [What's in this version?](#whats-in-this-version)
6. [What's next?](#whats-next)
7. [Links](#links)

# Code Engine

Code Engine is a simple, yet powerful, code execution engine. It is designed to be used in applications, where you want to empower your business users to maintain fragments of the application logic in a secure and controlled environment.

# Why Code Engine?

Code Engine allows a simple integration of code execution in your web application. It is designed to be secure and easy to use. It is also designed to be easily extendable, so you can add your own implementations to it.

For brand new products, adding Code Engine is as easy as adding the NuGet package to your solution, adding some initialization code, injecting specific "executors" throughout your application, and either providing hard-coded logic for their implementation or handing the responsibility to maintain that custom logic to your business users.

For existing products, you can use Code Engine to replace some of the existing logic with a more flexible and maintainable solution. It is easy to incorporate into ane existing application because it works with the existing dependency injection framework in ASP.NET Core.

Code Engine is as close to the performance of the compiled and deployed code as it gets. It is designed to be fast and efficient, to be used in a multi-threaded environment.

# How to use it?

1. Start with an existing solution or brand new
2. Look up Nuget packages for "armatsoftware.code.engine"
3. Add the main package `com.armatsoftware.code.engine`
4. Add supplemental packages, if needed. Ex: `com.armatsoftware.code.engine.storage.file`
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
        public string SaySomething(IExecutor<SubjectModel> messageGenerator, SubjectModel model)
        {
            messageGenerator.Subject = model;
            messageGenerator.Execute();
            return model.Message;
        }
    }
    ```

# Versions
- 1.x.x - essential contracts and base implementation for injection, compilation and execution of the custom logic, including initialization method and implementations for the file storage and file logger
- 2.x.x - added keyed executor lookup and rafactored file storage


# What's in this version?

The major update of this version is the addition of the "keyed" resolution of specific `IExecutor<T>` instances for the same subject type. This means that in a multi-tenant or multi-user application certain portion of application logic may be maintained by and used by specific users or tenants only. Please note that the existing direct resolution of `IExecutor<T>` is also available and hasn't changed.

``` c#
// direct injection of executor
public IActionResult Subject([FromServices] IExecutor<SubjectModel> executor)
{
    var model = new SubjectModel();
    executor.Subject = model;
    executor.Execute();
    return View(new SubjectModel() { Data = model.Data });
}

// injection of the catalog and executor selection by key
public IActionResult Subject([FromServices] IExecutorCatalog<SubjectModel> executors, string key = "")
{
    var model = new SubjectModel();
    var executor = executors.ForKey(key);
    executor.Subject = model;
    executor.Execute();
    return View(new SubjectModel() { Data = model.Data });
}
```

# What's next?

I am looking at a number of improvements:

- Visibility into the action execution: logging, metrics, etc.
- Traceability of the action execution: tracing, etc.
- User-friendly editor for the actions: development, testing, etc.


# Links

[Project Web Site](https://armatsoftware.com/code-engine/)