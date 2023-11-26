using Microsoft.Extensions.Logging;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using ValMati.StockBot.Providers.Abstractions;
using ValMati.StockBot.Providers.Model;
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
        IProvider provider = Substitute.For<IProvider>();
        MessageHandler sut = new MessageHandler(logger, provider);

        // Act
        await sut.HandleUpdateAsync(botClient, update, CancellationToken.None);

        // Assert
        await botClient
                .DidNotReceiveWithAnyArgs()
                .SendTextMessageAsync(chatId: default!, text: default!, cancellationToken: default);

        await provider
                .DidNotReceiveWithAnyArgs()
                .GetDataAsync(symbol: default!);
    }

    [Fact]
    public async Task HandleUpdateAsync_MessageIsText()
    {
        // Arrange
        ITelegramBotClient botClient = Substitute.For<ITelegramBotClient>();

        Update update = new Update { Message = new Message { Text = null } };

        ILogger<MessageHandler> logger = Substitute.For<ILogger<MessageHandler>>();
        IProvider provider = Substitute.For<IProvider>();
        MessageHandler sut = new MessageHandler(logger, provider);

        // Act
        await sut.HandleUpdateAsync(botClient, update, CancellationToken.None);

        // Assert
        await botClient
                .DidNotReceiveWithAnyArgs()
                .SendTextMessageAsync(chatId: default!, text: default!, cancellationToken: default);

        await provider
                .DidNotReceiveWithAnyArgs()
                .GetDataAsync(symbol: default!);
    }

    [Fact]
    public async Task HandleUpdateAsync_OK()
    {
        // Arrange
        Random random = new Random();
        long chatId = random.NextInt64();
        string textMessage = Guid.NewGuid().ToString();
        CancellationToken cancellationToken = new CancellationToken();

        ITelegramBotClient botClient = Substitute.For<ITelegramBotClient>();

        Update update = new Update
        {
            Message = new Message
            {
                Text = textMessage,
                Chat = new Chat
                {
                    Id = chatId
                }
            }
        };

        ILogger<MessageHandler> logger = Substitute.For<ILogger<MessageHandler>>();

        IProvider provider = Substitute.For<IProvider>();
        provider.GetDataAsync(textMessage).Returns(new Data());

        MessageHandler sut = new MessageHandler(logger, provider);

        // Act
        await sut.HandleUpdateAsync(botClient, update, CancellationToken.None);

        // Assert
        await botClient
                .Received(1)
                .MakeRequestAsync(
                    Arg.Is<SendMessageRequest>(r => r.ChatId == chatId && r.Text.Contains(textMessage)),
                    cancellationToken);

        await provider
                .Received(1)
                .GetDataAsync(textMessage);
    }

    [Fact]
    public async Task HandlePollingErrorAsync_LogError()
    {
        // Arrange
        ITelegramBotClient botClient = Substitute.For<ITelegramBotClient>();

        string exceptionMessage = Guid.NewGuid().ToString();
        ApiRequestException exception = new ApiRequestException(exceptionMessage);

        ILogger<MessageHandler> logger = Substitute.For<ILogger<MessageHandler>>();
        IProvider provider = Substitute.For<IProvider>();
        MessageHandler sut = new MessageHandler(logger, provider);

        // Act
        await sut.HandlePollingErrorAsync(botClient, exception, CancellationToken.None);

        // Assert
        logger
            .Received(1)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                exception,
                Arg.Any<Func<object, Exception?, string>>());

        await provider
                .DidNotReceiveWithAnyArgs()
                .GetDataAsync(symbol: default!);
    }
}