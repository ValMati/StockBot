using Microsoft.Extensions.Logging;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Types;
using ValMati.StockBot.Services;

namespace ValMati.StockBot.Tests.Services;

public class MessageHandlerTest
{
    [Fact]
    public async Task HandleUpdateAsync_NotAMessage()
    {
        // Arrange
        ITelegramBotClient botClient = Substitute.For<ITelegramBotClient>();

        Update update = new Update { Message = null };

        ILogger<MessageHandler> logger = Substitute.For<ILogger<MessageHandler>>();
        MessageHandler sut = new MessageHandler(logger);

        // Act
        await sut.HandleUpdateAsync(botClient, update, CancellationToken.None);

        // Assert
        await botClient.DidNotReceiveWithAnyArgs().SendTextMessageAsync(chatId: default!, text: default!, cancellationToken: default);
    }
}