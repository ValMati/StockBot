using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using ValMati.StockBot.Services.Abstractions;

namespace ValMati.StockBot.Test;

public class AppConsoleRunnerTest
{
    [Fact]
    public async Task HandleUpdateAsync()
    {
        // Arrange
        IMessageHandler messageHandler = Substitute.For<IMessageHandler>();
        ITelegramBotClient telegramBotClient = Substitute.For<ITelegramBotClient>();

        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(telegramBotClient);
        services.AddScoped<IMessageHandler>(_ => messageHandler);
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        ITelegramBotClient botClient = Substitute.For<ITelegramBotClient>();
        Update update = new Update { Message = new Message { Text = Guid.NewGuid().ToString() } };
        CancellationToken cancellationToken = new CancellationToken();

        AppConsoleRunner sut = new AppConsoleRunner(serviceProvider);

        // Act
        await sut.HandleUpdateAsync(botClient, update, cancellationToken);

        // Assert
        await messageHandler
                    .Received(1)
                    .HandleUpdateAsync(botClient, update, cancellationToken);
    }

    [Fact]
    public async Task HandlePollingErrorAsync()
    {
        // Arrange
        IMessageHandler messageHandler = Substitute.For<IMessageHandler>();
        ITelegramBotClient telegramBotClient = Substitute.For<ITelegramBotClient>();

        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(telegramBotClient);
        services.AddScoped<IMessageHandler>(_ => messageHandler);
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        ITelegramBotClient botClient = Substitute.For<ITelegramBotClient>();
        Exception exception = new ApiRequestException(Guid.NewGuid().ToString());
        CancellationToken cancellationToken = new CancellationToken();

        AppConsoleRunner sut = new AppConsoleRunner(serviceProvider);

        // Act
        await sut.HandlePollingErrorAsync(botClient, exception, cancellationToken);

        // Assert
        await messageHandler
                    .Received(1)
                    .HandlePollingErrorAsync(botClient, exception, cancellationToken);
    }

}
