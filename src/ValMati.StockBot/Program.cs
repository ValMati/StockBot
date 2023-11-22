using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using ValMati.StockBot.Services;
using ValMati.StockBot.Services.Abstractions;

namespace ValMati.StockBot;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
#if DEBUG
            .AddUserSecrets(typeof(Program).Assembly)
#else
            .AddEnvironmentVariables()
#endif
    .Build();

        Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console(
                                restrictedToMinimumLevel: LogEventLevel.Information,
                                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level}] - {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();

        try
        {
            // Create service collection
            IServiceCollection services = new ServiceCollection();

            // Setup dependencies
            services.AddLogging(
                loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(dispose: true, logger: Log.Logger);
                });

            services.AddSingleton<ITelegramBotClient>(_ =>
            {
                string token = configuration["BotConfig:Token"]!;
                return new TelegramBotClient(token);
            });

            services.AddScoped<IMessageHandler, MessageHandler>();

            // Build service provider
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Run the console app
            AppConsoleRunner appConsoleRunner = new(serviceProvider);
            await appConsoleRunner.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}