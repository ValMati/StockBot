using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

IConfigurationRoot configuration = new ConfigurationBuilder()
#if DEBUG
    .AddUserSecrets(typeof(Program).Assembly)
#else
    .AddEnvironmentVariables()
#endif
    .Build();

Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                    .CreateLogger();

string token = configuration["BotConfig:Token"]!;

TelegramBotClient botClient = new(token);

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

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
    {
        return;
    }

    // Only process text messages
    if (message.Text is not { } messageText)
    {
        return;
    }

    long chatId = message.Chat.Id;

    Log.Information("Received a '{messageText}' message in chat {chatId}.", messageText, chatId);

    // Echo received message text
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "You said:\n" + messageText,
        cancellationToken: cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    string? errorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Log.Error(exception, errorMessage);

    return Task.CompletedTask;
}
