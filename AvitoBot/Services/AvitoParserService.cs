using PuppeteerSharp;

namespace AvitoBot.Services;

public class AvitoParserService
{
    public async Task<string> GetAveragePriceAsync(string model, string selectedCity)
    {
        try
        {
            Console.WriteLine($"[Парсер] Начинаю поиск для: {model}");

            var options = new LaunchOptions
            {
                Headless = true, 
                ExecutablePath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            };

            Console.WriteLine("[Парсер] Запускаю браузер...");
            await using var browser = await Puppeteer.LaunchAsync(options);
    
            Console.WriteLine("[Парсер] Открываю страницу...");
            await using var page = await browser.NewPageAsync();
            string cityCode = GetCityUrl(selectedCity);
            string query = Uri.EscapeDataString(model);
            string url = $"https://www.avito.ru/{cityCode}?q={query}&s=104";            
            Console.WriteLine($"[Парсер] Ищу в городе: {selectedCity} ({cityCode})");
            await page.GoToAsync(url, new NavigationOptions 
            { 
                Timeout = 60000, 
                WaitUntil = new[] { WaitUntilNavigation.Load } 
            });
            Console.WriteLine("--- Данные получены ---");
            
            var prices = await page.EvaluateFunctionAsync<int[]>(@"() => {
                const elements = document.querySelectorAll('meta[itemprop=""price""]');
                return Array.from(elements).map(el => parseInt(el.getAttribute('content'))).filter(p => p > 0);
            }");

            if (prices == null || prices.Length == 0)
                return "🔍 Цены не найдены. Авито все еще сопротивляется.";

            int avgPrice = (int)prices.Average();
            int minPrice = prices.Min();

            return $"📊 *Анализ через Browser:* {model}\n" +
                   $"──────────────────\n" +
                   $"📉 Мин: `{minPrice:N0} ₽` | Ср: `{avgPrice:N0} ₽`";
        }
        catch (Exception ex)
        {
            return $"⚠️ Ошибка браузера: {ex.Message}";
        }
    }

    public string  GetCityUrl( string selectedCity)
    {
        return selectedCity switch
        {
            "Москва" => "moskva",
            "Санкт-Петербург" => "sankt-peterburg",
            "Пятигорск" => "pyatigorsk",
            "Ессентуки" => "essentuki",
            "Кисловодск" => "kislovodsk",
            _ => "rossiya"
        };
    }
}