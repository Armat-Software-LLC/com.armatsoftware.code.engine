using ArmatSoftware.Code.Engine.Compiler.DI;
using ArmatSoftware.Code.Engine.Logger.File;
using ArmatSoftware.Code.Engine.Tester.WebApi;

var builder = WebApplication.CreateBuilder(args);

// add controllers
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.UseCodeEngine(new()
{
    CompilerType = CompilerTypeEnum.CSharp,
    CodeEngineNamespace = "com.armatsoftware.code.engine.executors",
    Logger = new CodeEngineFileLogger("./test.log"),
    Storage = new CustomCodeStorage()
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