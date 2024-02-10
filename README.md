# Code Engine

Code Engine is a simple, yet powerful, code execution engine. It is designed to be used in applications, where you want to empower your business users to maintain fragments of the application logic in a secure and controlled environment.

# Why Code Engine?

Code Engine allows a simple integration of code execution in your web application. It is designed to be secure and easy to use. It is also designed to be easily extendable, so you can add your own implementations to it.

For brand new products, adding Code Engine is as easy as adding the NuGet package to your solution, adding some initialization code, injecting specific "executors" throughout your application, and either providing hard-coded logic for their implementation or handing the responsibility to maintain that custom logic to your business users.

For existing products, you can use Code Engine to replace some of the existing logic with a more flexible and maintainable solution. It is easy to incorporate into ane existing application because it works with the existing dependency injection framework in ASP.NET Core.

Code Engine is as close to the performance of the compiled and deployed code as it gets. It is designed to be fast and efficient, to be used in a multi-threaded environment.

# What's in this version?

First, I separated the provision of the actions for the compiler from the management of actions. There is a default implementation for the `IActionProvider` contract that works with the default implementation of the `IActionRepository` contract - the management interface of the actions using file system.

# What's next?

I am looking at a number of improvements:

- Visibility into the action execution: logging, metrics, etc.
- Traceability of the action execution: tracing, etc.
- User-friendly editor for the actions: development, testing, etc.


# Links

[Project Web Site](https://armatsoftware.com/code-engine/)