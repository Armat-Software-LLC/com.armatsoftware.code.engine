
# Code Engine Storage

## Table of Contents

1. [Code EngineStorage](##code-engine-storage)
2. [Benefits](##benefits)
3. [How to use it?](##how-to-use-it)

## Code Engine Storage

Code Engine Storage is a storage implementation for the Code Engine. Its primary purpose is to create protection mechanism against data orrution and to provide a way to store and retrieve the custom logic in a secure and controlled environment.

## Benefits

### Data Completeness

Specific subject action model defintions and their validation ensure completeness of the created actions and their data.

### Data Integrity

Stored subject action data model hierarchy and rules within the implementation ensure the integrity of the stored data.

### Versioning

The storage implementation provides versioning for the stored subject actions, allowing for safe creation and updates as well as "release" of specific versions of the stored subject action.

An update of a stored subject action creates a new version of the action that is not yet active, allowing for safe continued use of prior revisions.

Once a revision (or version) of a stored subject action is activated, it becomes the current version and is used by the code engine provider going forward. Depending on the caching configuration, the current version may still be cached and effective for a specific period of time until its expiration.

Ideally, the responsibility of creating and updating stored subject actions should be separated from the responsibility of activating one or another version.

### Ease of Use

With the default storage implementation, you only need to choose between using one of the provided storage adapters or provide your own custom implementation for one.

## How to use it?

1. Start with an existing solution or brand new.
1. You should already have the main package `com.armatsoftware.code.engine` added to your solution and `UseCodeEngine()` initialization set up.
1. Look up NuGet packages for `armatsoftware.code.engine.storage`.
1. Add the main package `com.armatsoftware.code.engine.storage` to your solution.
1. Include initialization logic for the default storage implementation. Provide your own custom implementation of the storage adapter or use one provided by Code Engine (look on NuGet). Ex:
    ``` c#
   builder.Services.UseCodeEngineStorage();
    ```
    or
    ``` c#
   builder.Services.UseCodeEngineStorage(new CustomStorageAdapter());
    ```
1. Inject the `IActionStorage` and use it as needed to manage the subject actions. Ex:
    ``` c#
     [HttpPost, Route("create")]
     public IEnumerable<ISubjectAction<SimpleSubject>> Create([FromServices] IActionStorage storage, [FromBody] SimpleSubjectUpdateModel updateModel)
     {
         storage.AddAction<SimpleSubject>(updateModel.Name, updateModel.Code, updateModel.Author, updateModel.Comment);
         return storage.GetActions<SimpleSubject>();
     }
    ```
1. Inject `IActionProvider` to retrieve the stored subject actions and use them in your code. Ex:
    ``` c#
     [HttpGet, Route("actions")]
     public string GetActions([FromServices] IActionProvider storage)
     {
         return string.Join(", ", storage.Retrieve<SimpleSubject>().Select(x => x.Name));
     }
    ```