# CRM & AI Assistant for Apple Resellers
Интеллектуальный Telegram-бот для автоматизации учета техники, мониторинга цен и генерации продающих описаний с помощью ИИ. Разработан как полноценная система управления для магазина электроники.

🚀 Основные функции
📦 Учет склада (SQLite): Добавление, просмотр и полная очистка базы данных товаров (модель, память, цена, состояние).

🤖 AI Copywriter (Groq/Llama 3.1): Автоматическая генерация привлекательных объявлений для Авито на основе характеристик устройства.

📊 Бизнес-аналитика: Расчет общей выручки по закупкам за последние 7 дней.

📈 Мониторинг рынка: Поиск средних цен на устройства в разных городах через веб-парсер.

💵 Курсы валют: Оперативное получение актуальных курсов для оценки стоимости импорта.

## Технологический стек
Язык: C# (.NET 8.0).

Библиотека бота: Telegram.Bot.

База данных: SQLite + Dapper (для быстрых и эффективных SQL-запросов).

Интеграция ИИ: Groq Cloud API (модель Llama 3.1).

Конфигурация: Microsoft.Extensions.Configuration (безопасное хранение ключей в JSON).

## Установка и запуск
Клонируйте репозиторий:

Bash
git clone https://github.com/yourusername/Avito-Bot.git
Настройте конфигурацию:
Создайте файл appsettings.json в корневой директории:

JSON
{
"BotConfiguration": {
"BotToken": "ВАШ_ТГ_ТОКЕН",
"GroqAPIKey": "ВАШ_GROQ_КЛЮЧ"
}
}

Запустите проект:

Bash

dotnet run

## Структура проекта
Services/ — логика бота, работа с ИИ, парсером и конфигами.

Models/ — описание сущностей (например, AppleProduct).

Data/ — репозитории для работы с SQLite.
### Developed by: **fewvar**


# English version

An intelligent Telegram bot for automating equipment accounting, price monitoring, and generating selling descriptions using AI. It is designed as a complete control system for an electronics store.

, Main functions
, Warehouse accounting (SQLite): Adding, viewing, and completely clearing the product database (model, memory, price, condition).

AI Copywriter (Groq/Llama 3.1): Automatic generation of attractive ads for Avito based on device characteristics.

📊 Business analytics: Calculation of the total revenue from purchases over the last 7 days.

📈 Market monitoring: Search for average device prices in different cities through a web parser.

💵 Exchange rates: Prompt receipt of up-to-date exchange rates for estimating the cost of imports.

## Technology stack
Language: C# (.NET 8.0).

Bot library: Telegram.Bot.

Database: SQLite + Dapper (for fast and efficient SQL queries).

AI integration: Groq Cloud API (Llama 3.1 model).

Configuration: Microsoft.Extensions.Configuration (secure key storage in JSON).

## Installation and launch
Clone the repository:

Bash
git clone https://github.com/yourusername/Avito-Bot.git
Configure the configuration:
Create the appsettings file.json in the root directory:

JSON
{
"BotConfiguration": {
"BotToken": "YOUR_TG_TOKEN",
"GroqAPIKey": "YOUR_GROQ_KEY"
}
}

Launch the project:

Bash

dotnet run

## Project structure
Services/ — bot logic, working with AI, parser, and configs.

Models/ — description of entities (for example, AppleProduct).

Data/ — repositories for working with SQLite.



### Developed by: **fewvar**