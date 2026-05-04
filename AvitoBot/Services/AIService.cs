using System.Text;
using System.Text.Json;

namespace AvitoBot.Services;

public class AiService
{
    
    private readonly string _apiKey = ConfigService.GetApiKey(); 
    private readonly string _apiUrl = "https://api.groq.com/openai/v1/chat/completions";

    public async Task<string> GenerateDescriptionAsync(string specs)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var requestBody = new
        {
            model = "llama-3.1-8b-instant", 
            messages = new[]
            {
                new { 
                    role = "system", 
                    content = "Ты — профи по продаже техники Apple на Авито. Пиши нативно, без лишней 'воды' и официальщины. " +
                        "Используй заголовок с моделью, разделяй текст на логические блоки: Состояние, Комплект, Аккумулятор. " +
                        "В конце добавь: 'Пишите/звоните, отвечу на все вопросы!' и пару смайликов. " +
                        "Не используй странные термины вроде 'без целибрата'."
                },
                new { role = "user", content = $"Создай объявление для: {specs}" }
            },
            temperature = 0.7 
        };

        try 
        {
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);
            
            var json = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Ошибка Groq: {json}");
                return "❌ Не удалось связаться с ИИ.";
            }

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "🤖 ИИ вернул пустой ответ.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в AIService: {ex.Message}");
            return "❌ Произошла ошибка при генерации текста.";
        }
    }
}