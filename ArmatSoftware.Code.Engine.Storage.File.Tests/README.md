
# Code Engine Storage File Adapter

## What is File Adapter?

Code Engine File Adapter is an out of the box implementation of the storage adapter `IStorageAdapter` contract to manage the subject actions on the file system. It stands between the storage abstraction layer and the persistence layer, whatever it may be: a relational database, blob storage, or file system. In this case, it is the file system.

`IStorageAdapter` is responsible for the CRUD operations on the subject actions and allows the abstraction layer focus on the action management.

## How to use it?

1. Include file adapter initialization logic to your `Startup` and supply necessary parameters, usually from `IConfiguration`. Ex:
    ``` c#
   services.UseCodeEngineFileAdapter(new FileStorageOptions()
   {
       FileExtension = "code",
       StoragePath = "/tmp/demo/"
   });
    ```
    `UseCodeEngineFileAdapter` also takes an optional `ServiceLifetime` (defaults to `Scoped`; pass
    `ServiceLifetime.Singleton` for hosts with no per-request DI scope).

## Choosing this adapter at runtime, without a compile-time reference

Hosts that select their storage adapter from configuration instead of a direct project reference (see
`ArmatSoftware.Code.Engine.Storage`'s README for the general pattern) can point at this package's
`FileStorageAdapterRegistrar` (`DI/FileStorageAdapterRegistrar.cs`), which implements
`ArmatSoftware.Code.Engine.Storage.DI.IStorageAdapterRegistrar` and reads a `FileStorage` configuration
section (`StoragePath`, `FileExtension`) to call `UseCodeEngineFileAdapter` for you:

``` json
{
  "FileStorage": {
    "StoragePath": "/tmp/demo/",
    "FileExtension": "code"
  }
}
```

A host loads it by assembly path + `ArmatSoftware.Code.Engine.Storage.File.DI.FileStorageAdapterRegistrar`
as the type full name - `ArmatSoftware.Code.Engine.LanguageServer` is one example of a host that works this
way (see its README's Configuration section).