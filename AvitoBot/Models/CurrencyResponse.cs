namespace AvitoBot.Models;

public class CurrencyResponse
{
    public Dictionary<string, decimal> rates { get; set; } = new();
}