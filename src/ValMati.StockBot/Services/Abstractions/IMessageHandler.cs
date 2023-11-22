using Telegram.Bot;
using Telegram.Bot.Types;

namespace ValMati.StockBot.Services.Abstractions;

internal interface IMessageHandler
{
    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);

    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
}
