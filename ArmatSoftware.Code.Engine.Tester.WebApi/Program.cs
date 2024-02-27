using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Logger.File;
using ArmatSoftware.Code.Engine.Storage.DI;
using ArmatSoftware.Code.Engine.Storage.File;
using ArmatSoftware.Code.Engine.Storage.File.DI;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// add controllers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            var result = new BadRequestObjectResult(errors);
            result.ContentTypes.Add("application/json");
            return result;
        };
    });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Specify that you are going to use code engine
// to enable the injection of the code engine implementations
builder.Services.UseCodeEngine(new()
{
    // compiler type dictates in which language your power users
    // will be writing the custom logic for code engine
    CompilerType = CompilerTypeEnum.CSharp,
    // specify unique namespace for the code engine no to mix with your code
    CodeEngineNamespace = "com.armatsoftware.code.engine.executors",
    // specify the custom code provider or don't set to use the default one
    Logger = new CodeEngineFileLogger("/tmp/test.log"),
    // likewise, optionally, use your own custom provider for the storage
    // or use default if none provided
    // Provider = new CustomCodeProvider()
    // set the cache expiration time in minutes
    CacheExpirationMinutes = 1
});

// set the default repository and storage
builder.Services.UseCodeEngineDefaultRepository();

builder.Services.UseCodeEngineDefaultFileStorage(new FileStorageOptions()
{
    StoragePath = config["ASCE_FILE_STORAGE_PATH"] ?? throw new ApplicationException("Unable to find ASCE_FILE_STORAGE_PATH in the configuration"),
    FileExtension = config["ASCE_FILE_STORAGE_EXTENSION"] ?? throw new ApplicationException("Unable to find ASCE_FILE_STORAGE_EXTENSION in the configuration")
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();