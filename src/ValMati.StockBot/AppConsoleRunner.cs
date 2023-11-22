using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ValMati.StockBot.Services.Abstractions;

namespace ValMati.StockBot;

internal class AppConsoleRunner
{
    private readonly IServiceProvider serviceProvider;

    public AppConsoleRunner(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task RunAsync()
    {
        Log.Information("Starting...");

        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();

        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        Log.Information("Start listening for @{username}", me.Username);

        await Task.Delay(Timeout.Infinite, cts.Token);
    }

    internal async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using IServiceScope serviceScope = serviceProvider.CreateScope();
        IServiceProvider scopedProvider = serviceScope.ServiceProvider;

        IMessageHandler messageHandler = scopedProvider.GetRequiredService<IMessageHandler>();

        await messageHandler.HandleUpdateAsync(botClient, update, cancellationToken);
    }

    internal async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        using IServiceScope serviceScope = serviceProvider.CreateScope();
        IServiceProvider scopedProvider = serviceScope.ServiceProvider;

        IMessageHandler messageHandler = scopedProvider.GetRequiredService<IMessageHandler>();

        await messageHandler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
    }
}




