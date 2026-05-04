using AvitoBot.Models;
using System.Net.Http.Json;

namespace AvitoBot.Services;

public class CurrencyService
{
    private readonly HttpClient _httpClient = new();
    
    public async Task<string> GetRatesAsync()
    {
        try
        {
            var url = "https://open.er-api.com/v6/latest/USD";
            
            var response = await _httpClient.GetFromJsonAsync<CurrencyResponse>(url);

            if (response == null || !response.rates.ContainsKey("RUB") || !response.rates.ContainsKey("CNY"))
            {
                return "❌ Ошибка: Не удалось получить данные от сервера валют.";
            }

            decimal usdToRub = response.rates["RUB"];
            decimal usdToCny = response.rates["CNY"];
            decimal cnyToRub = usdToRub / usdToCny;

            return $"📊 *EXCHANGE*\n" +
                   $"──────────────────\n" +
                   $"🇺🇸 `USD:` *{usdToRub:F2} ₽*\n" +
                   $"🇨🇳 `CNY:` *{cnyToRub:F2} ₽*\n" +
                   $"──────────────────\n" +
                   $"_🕒 {DateTime.Now:HH:mm}_";
        }
        catch (Exception ex)
        {
            return $"⚠️ Не удалось обновить курс: {ex.Message}";
        }
    }
}