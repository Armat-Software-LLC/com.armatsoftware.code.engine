
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