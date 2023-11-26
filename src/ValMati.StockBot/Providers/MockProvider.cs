using ValMati.StockBot.Providers.Abstractions;
using ValMati.StockBot.Providers.Model;

namespace ValMati.StockBot.Providers;

internal class MockProvider : IProvider
{
    public Task<Data> GetDataAsync(string symbol)
    {
        Data result = new Data
        {
            Currency = "EUR",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            Open = 75.5m,
            Close = 78.6m,
            Maximum = 100.0m,
            Minimum = 65.4m,
            Volume = 7896.45m,
            AdditionalInfo = "This is mock information."
        };

        return Task.FromResult(result);
    }
}
