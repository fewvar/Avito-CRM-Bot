# Avito-CRM-Bot

Telegram-бот на C# для учёта техники: склад в SQLite, генерация текстов объявлений через LLM, средние рыночные цены по городам и курсы валют.

> Архивный проект, не развивается.

## Что умеет

- **Склад** — добавление, просмотр и очистка базы товаров: модель, память, цена, состояние
- **Тексты объявлений** — генерация описания по характеристикам устройства через Groq (Llama 3.1)
- **Аналитика** — сумма закупок за последние 7 дней
- **Цены** — поиск средней цены на устройство в выбранном городе
- **Курсы валют** — для оценки стоимости ввоза

## Стек

C# / .NET 8, Telegram.Bot, SQLite + Dapper, Groq API, Microsoft.Extensions.Configuration

## Запуск

```bash
git clone https://github.com/fewvar/Avito-CRM-Bot.git
cd Avito-CRM-Bot/AvitoBot
cp appsettings.Example.json appsettings.json   # вписать токен бота и ключ Groq
dotnet run
```

## Структура

```
AvitoBot/
  Services/   бот, LLM, парсер цен, курсы, конфиг
  Models/     сущности (AppleProduct, CurrencyResponse)
  Data/       работа с SQLite
```
