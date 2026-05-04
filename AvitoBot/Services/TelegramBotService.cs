using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using AvitoBot.Data;
using AvitoBot.Models;

namespace AvitoBot.Services;

public class TelegramBotService
{
    
    private readonly CurrencyService _currencyService = new();
    private readonly AvitoParserService _avitoService = new();
    private static readonly Dictionary<long, int> UserStates = new();
    private static readonly Dictionary<long, string> UserCities = new();
    private readonly ProductRepository _productRepository = new();
    private readonly AiService _aiService = new();
    
    public  async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {if (update.Type == UpdateType.CallbackQuery)
        {
            var callback = update.CallbackQuery;
            var cchatId = callback.Message.Chat.Id;

            if (callback.Data == "db_list")
            {
                var products = _productRepository.GetAllProducts(); 

                if (!products.Any())
                {
                    await bot.SendMessage(cchatId, "📭 База пока пуста.");
                }
                else
                {
                    var text = "📋 **Текущий склад:**\n\n" + string.Join("\n", products.Select(p => 
                        $"📱 {p.Model} {p.Storage}GB\n💰 {p.Price}₽ | {(p.IsNew == 1 ? "Новый" : "Б/У")}\n📅 {p.CreatedAt}\n---"));
                
                    await bot.SendMessage(cchatId, text, parseMode: ParseMode.Markdown);
                }
            }
            
            if (callback.Data == "db_add")
            {
                UserStates[cchatId] = 2; 
                await bot.SendMessage(
                    chatId: cchatId, 
                    text: "📝 Введите данные через запятую:\n*Модель, Цена, Память, Новый(1/0)*\n\nПример: `iPhone 15, 75000, 128, 1`", 
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
            }

            if (callback.Data == "db_revenue_7")
            {
                var total = _productRepository.GetTotalRevenue(7);
                await bot.SendMessage(
                    chatId: cchatId, 
                    text: $"📊 **Итог по закупкам за 7 дней:**\nОбщая сумма: `{total:N0}₽`", 
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
            }
            
            if (callback.Data == "db_clear")
            {
                 _productRepository.ClearAll(); 
                await bot.SendMessage(cchatId, "⚠️ Тестовая очистка базы выполнена (если метод реализован).");
            }
        
            await bot.AnswerCallbackQuery(callback.Id, cancellationToken: ct);
            return; 
        }
        
        
        if (update.Message is not { Text: { } messageText } message) return;

    var chatId = message.Chat.Id;
    
    Console.WriteLine($"[Сообщение] {message.From?.FirstName}: {messageText} {DateTime.Now}");
    
    UserStates.TryGetValue(chatId, out int state);
    
    if (state == 1)
    {
        string[] commands = { "⬅️ Назад", "📈 Курс валют", "📍 Выбрать город", "💰 Учет", "📝 AI Описание" };
    
        if (commands.Contains(messageText))
        {
            UserStates[chatId] = 0; 
        }
        else 
        {
            if (!UserCities.TryGetValue(chatId, out var currentCity))
            {
                currentCity = "Россия"; 
            }

            await bot.SendMessage(chatId, $"🔍 Ищу среднюю цену на: {messageText} (Город: {currentCity})...", cancellationToken: ct);
            var result = await _avitoService.GetAveragePriceAsync(messageText, currentCity);
            await bot.SendMessage(chatId, result, parseMode: ParseMode.Markdown, cancellationToken: ct);

            UserStates[chatId] = 0; 
            await ShowMainMenu(bot, chatId, ct);
            return;
        }
    }
    
    if (state == 2)
    {
        try 
        {
            var data = messageText.Split(',');
            var product = new AppleProduct
            {
                Model = data[0].Trim(),
                Price = int.Parse(data[1].Trim()),
                Storage = int.Parse(data[2].Trim()),
                IsNew = int.Parse(data[3].Trim()),
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _productRepository.AddProduct(product);
            await bot.SendMessage(chatId, "✅ Товар успешно добавлен в SQLite!");
        }
        catch 
        {
            await bot.SendMessage(chatId, "❌ Ошибка формата! Попробуйте еще раз или нажмите /start для отмены.");
        }
        
        UserStates[chatId] = 0;
        await ShowMainMenu(bot, chatId, ct);
        return;
    }
    
    if (state == 3)
    {
        await bot.SendMessage(chatId, "⏳ **WEAPPLE AI** генерирует описание...", parseMode: ParseMode.Markdown, cancellationToken: ct);
    
        var aiText = await _aiService.GenerateDescriptionAsync(messageText);
    
        UserStates[chatId] = 0;
    
        await bot.SendMessage(chatId, aiText, cancellationToken: ct);
        await ShowMainMenu(bot, chatId, ct);
        return;
    }

    switch (messageText)
    {
        case "/start":
            await ShowMainMenu(bot, chatId, ct);
            break;

        case "📊 Мониторинг цен":
            UserStates[chatId] = 1;
            var cityKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "📍 Выбрать город" },
                new KeyboardButton[] { "⬅️ Назад" } 
            })
            {
                ResizeKeyboard = true 
            };

            await bot.SendMessage(
                chatId: chatId, 
                text: "Выберите город через меню или сразу введите модель устройства (поиск будет по России):", 
                replyMarkup: cityKeyboard, 
                cancellationToken: ct);
            break;
        case "⬅️ Назад" :
            await ShowMainMenu(bot, chatId, ct);
            break;

        case "📍 Выбрать город":
            var locationKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("Москва"), new KeyboardButton("Санкт-Петербург") },
                new[] { new KeyboardButton("Кисловодск"), new KeyboardButton("Пятигорск") },
                new[] { new KeyboardButton("Ессентуки"), new KeyboardButton("Россия") }
            }) { ResizeKeyboard = true };

            await bot.SendMessage(chatId, "В каком городе ищем технику?", replyMarkup: locationKeyboard, cancellationToken: ct);
            break;

        case "Москва":
        case "Кисловодск":
        case "Пятигорск":
        case "Ессентуки":
        case "Санкт-Петербург":
        case "Россия":
            UserCities[chatId] = messageText;
            UserStates[chatId] = 1; 
            await bot.SendMessage(chatId, $"📍 Город изменен на: {messageText}. Жду название модели...", cancellationToken: ct);
            break;
        case "📈 Курс валют":
            var report = await _currencyService.GetRatesAsync();
            await bot.SendMessage(chatId, report, parseMode: ParseMode.Markdown, cancellationToken: ct);
            break;

        case "💰 Учет":
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("📦 Показать склад", "db_list") },
                new[] { InlineKeyboardButton.WithCallbackData("➕ Добавить запись", "db_add") },
                new[] { InlineKeyboardButton.WithCallbackData("🗑 Очистить (тест)", "db_clear") },
                new[] { InlineKeyboardButton.WithCallbackData("💰 Выручка (7 дн.)", "db_revenue_7") }
            });

            await bot.SendMessage(chatId, "🗄 **База данных WEAPPLE**\nВыберите действие с SQLite:", 
                replyMarkup: inlineKeyboard, parseMode: ParseMode.Markdown, cancellationToken: ct);
            break;

        case "📝 AI Описание":
            UserStates[chatId] = 3;
            await bot.SendMessage(chatId, "🤖 Отправьте характеристики (например: iPhone 13, 128гб, идеал, аккум 90%), и я сделаю описание.", cancellationToken: ct);
            break;

        default:
            await bot.SendMessage(chatId, "Я пока не знаю такой команды. Выбери что-то в меню ниже 👇", cancellationToken: ct);
            break;
    }
    }
    
    async Task ShowMainMenu(ITelegramBotClient bot, long chatId, CancellationToken ct)
    {
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "📈 Курс валют", "📝 AI Описание" },
            new KeyboardButton[] { "📊 Мониторинг цен", "💰 Учет" },
        })
        {
            ResizeKeyboard = true 
        };

        await bot.SendMessage(
            chatId: chatId,
            text: "👋 Привет! Это админка WEAPPLE.",
            replyMarkup: replyKeyboard,
            cancellationToken: ct
        );
    }
}