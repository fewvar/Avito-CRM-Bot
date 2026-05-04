using Microsoft.Extensions.Configuration;

namespace AvitoBot.Services;

public static class ConfigService
{
    private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) 
        .AddJsonFile("appsettings.json")
        .Build();

    public static string GetToken()
    {
        string? token = Config["BotConfiguration:BotToken"];
        if (string.IsNullOrEmpty(token)) throw new Exception("Ошибка: BotToken не найден!");
        return token;
    }

    public static string GetApiKey()
    {
        string? api = Config["BotConfiguration:GroqAPIKey"]; 

        if (string.IsNullOrEmpty(api))
        {
            throw new Exception("Ошибка: Ключ API (GroqAPIKey) не найден в appsettings.json!");
        }

        return api;
    }
}