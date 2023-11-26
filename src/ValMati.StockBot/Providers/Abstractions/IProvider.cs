using ValMati.StockBot.Providers.Model;

namespace ValMati.StockBot.Providers.Abstractions;

internal interface IProvider
{
    Task<Data> GetDataAsync(string symbol);
}
