using FluentAssertions;
using ValMati.StockBot.Providers;

namespace ValMati.StockBot.Test.Providers;

public class MockProviderTest
{
    [Fact]
    public async Task GetDataAsync_Ok()
    {
        // Arrange
        MockProvider sut = new();

        // Act
        var actual = await sut.GetDataAsync(Guid.NewGuid().ToString());

        // Assert
        actual.Should().NotBeNull();
    }
}
