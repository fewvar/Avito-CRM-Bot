using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using AvitoBot.Data;
using AvitoBot.Services;

string token = ConfigService.GetToken();
var botService = new TelegramBotService();
var botClient = new TelegramBotClient(token);

DbInitializer.Initialize();

using var cts = new CancellationTokenSource();


botClient.StartReceiving(
    updateHandler: botService.HandleUpdateAsync,
    errorHandler: HandlePollingErrorAsync,
    receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
    cancellationToken: cts.Token
);
 
Console.WriteLine("🚀 Бот запущен и готов к работе");
Console.ReadLine();
cts.Cancel();
 

async Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
{
    Console.WriteLine("Ошибка при опросе сервера: " + ex.Message);
    await Task.CompletedTask;
}   

