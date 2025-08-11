using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using ArmatSoftware.Code.Engine.LanguageServer.Services;

namespace ArmatSoftware.Code.Engine.LanguageServer;

class Program
{
    static async Task Main(string[] args)
    {
        var server = await OmniSharp.Extensions.LanguageServer.Server.LanguageServer.From(options =>
            options
                .WithInput(Console.OpenStandardInput())
                .WithOutput(Console.OpenStandardOutput())
                .ConfigureLogging(x => x
                    .AddLanguageProtocolLogging()
                    .SetMinimumLevel(LogLevel.Debug))
                .WithServices(ConfigureServices)
        );

        await server.WaitForExit;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICodeEngineAnalysisService, CodeEngineAnalysisService>();
        services.AddSingleton<ICompletionService, CompletionService>();
        services.AddSingleton<IDiagnosticsService, DiagnosticsService>();
        services.AddSingleton<ISymbolService, SymbolService>();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
    }
}