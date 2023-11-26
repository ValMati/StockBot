using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using ValMati.StockBot.Providers.Abstractions;
using ValMati.StockBot.Providers.Model;
using ValMati.StockBot.Services.Abstractions;

namespace ValMati.StockBot.Services;

internal class MessageHandler : IMessageHandler
{
    private readonly ILogger<MessageHandler> logger;
    private readonly IProvider provider;

    public MessageHandler(
        ILogger<MessageHandler> logger,
        IProvider provider)
    {
        this.logger = logger;
        this.provider = provider;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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

        logger.LogInformation("Received a '{messageText}' message in chat {chatId}.", messageText, chatId);

        Data data = await provider.GetDataAsync(messageText);

        string text = GetText(messageText, data);

        // Echo received message text
        _ = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            cancellationToken: cancellationToken);
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string? errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogError(exception, errorMessage);

        return Task.CompletedTask;
    }

    private string GetText(string messageText, Data data) =>
        $"Symbol: {messageText}\n" +
        $"Date: {data.Date}\n" +
        $"Open: {data.Open}\n" +
        $"Close: {data.Close}\n" +
        $"Max: {data.Maximum}\n" +
        $"Min: {data.Minimum}\n" +
        $"Vol: {data.Volume}";
}
