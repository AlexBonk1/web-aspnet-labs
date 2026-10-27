# WEB_453504_ASP_NET — Разработка Web-приложений на ASP.NET Core

Многоуровневое веб-приложение (электронный каталог музыкальных инструментов), разработанное в рамках лабораторных и курсовых работ по дисциплине «Разработка Web-приложений на ASP.NET Core» (группа 453504).

---

## Архитектура проекта и структура решения

Решение (`WEB_453504_ASP_NET.slnx`) построено по принципам многослойной архитектуры и включает следующие проекты:

1. **`WEB_453504_ASP_NET.Domain`**  
   Доменный слой (бизнес-логика, сущности, модели данных, контракты/интерфейсы).
2. **`WEB_453504_ASP_NET.API`**  
   RESTful Web API бэкенд. Обеспечивает работу с базой данных PostgreSQL, хранение кэша (Redis), аутентификацию и авторизацию через Keycloak, а также реализует паттерн CQRS (MediatR) и Minimal API эндпоинты.
3. **`WEB_453504_ASP_NET.UI`**  
   Основной MVC-интерфейс приложения. Взаимодействует с API через HttpClient, поддерживает сессионную корзину (`SessionCart`), кастомную аутентификацию/авторизацию с интеграцией Keycloak, логирование через Serilog и middleware для обработки ошибок. Содержит защищенную административную панель (Razor Pages).
4. **`WEB_453504_ASP_NET.Blazor.SSR`**  
   Вариант фронтенда на Blazor с Server-Side Rendering (SSR).
5. **`WEB_453504_ASP_NET.BlazorWasm`**  
   Клиентское Blazor WebAssembly приложение.
6. **`WEB_453504_ASP_NET.Tests`**  
   Проект с модульными и интеграционными тестами (xUnit / Moq / FluentAssertions).

---

## Технологический стек

- **Платформа:** .NET 10 / ASP.NET Core
- **База данных:** PostgreSQL 18+ (Entity Framework Core)
- **Кэширование:** Redis, HybridCache
- **Аутентификация и Авторизация:** OAuth 2.0 / OpenID Connect (Keycloak, JWT Bearer)
- **Паттерны и Библиотеки:** CQRS (MediatR), Minimal API + Controllers, Serilog (структурированное логирование), xUnit
- **UI & Фронтенд:** ASP.NET Core MVC, Razor Pages, Blazor (SSR & WebAssembly), Bootstrap / Open Iconic

---

## Начало работы и запуск

### Предварительные требования
- Установленный [.NET 10 SDK](https://dotnet.microsoft.com/)
- Установленный Docker / Docker Compose (для поднятия PostgreSQL, Redis и Keycloak, если требуется)

### Настройка конфигурации
Убедитесь, что файлы `appsettings.json` в проектах `API` и `UI` содержат корректные строки подключения к базе данных, Redis и параметры подключения к серверу авторизации Keycloak.

### Запуск решения
Вы можете запустить проект через Visual Studio, Rider или с помощью терминала .NET CLI:

1. **Запуск API:**
   ```bash
   dotnet run --project WEB_453504_ASP_NET.API
   ```
2. **Запуск UI (MVC):**
   ```bash
   dotnet run --project WEB_453504_ASP_NET.UI
   ```
3. **Запуск тестов:**
   ```bash
   dotnet test
   ```

---

