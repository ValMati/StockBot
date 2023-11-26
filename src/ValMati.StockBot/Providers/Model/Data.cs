namespace ValMati.StockBot.Providers.Model;

internal sealed record Data
{
    public string Currency { get; init; } = "EUR";

    public DateOnly Date { get; init; }

    public Decimal Open { get; init; }

    public Decimal Close { get; init; }

    public Decimal Maximum { get; init; }

    public Decimal Minimum { get; init; }

    public Decimal Volume { get; init; }

    public string AdditionalInfo { get; init; } = null!;
}
