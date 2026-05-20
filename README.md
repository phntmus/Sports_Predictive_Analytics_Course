# 424 Sport Predict

Веб-приложение предиктивной аналитики спортивных событий.

**Вариант 24 · Порт 8024 · SQLite · .NET 8 · Blazor Server**

## Стек технологий

- **ASP.NET Core 8** + **Blazor Server** — интерактивный веб-интерфейс
- **Entity Framework Core 8** (Code First) + **SQLite** — база данных 
- **ASP.NET Core Identity** — аутентификация и авторизация (18+)
- **FluentValidation** — валидация форм
- **Docker** — контейнеризация (вариант 24, порт 8024)
- **xUnit** — unit-тесты

## Структура проекта

```
SportsPredictiveAnalyticsCoursework/
├── src/
│   └── SportsAnalyticsWeb/
│       ├── Components/         # Blazor страницы и компоненты
│       │   ├── Pages/          # Home, Calculate, Teams, Matches, Odds, Login, Register
│       │   └── Layout/         # MainLayout, App, Routes
│       ├── Controllers/        # AuthController (вход/регистрация/выход)
│       ├── Data/               # DbContext, Migrations, DbInitializer
│       ├── Models/             # EF Core сущности (8 классов)
│       ├── Repositories/       # Паттерн Repository
│       ├── Services/           # OddsPredictionService (математическая модель)
│       ├── Validators/         # FluentValidation (включая проверку возраста 18+)
│       ├── ViewModels/         # DTO-классы
│       └── wwwroot/
│           ├── css/app.css     # Стили (чёрно-красно-белая схема)
│           └── img/crests/     # SVG-гербы команд
├── tests/
│   └── SportsAnalytics.Tests/ # xUnit unit-тесты
├── docker-compose.yml
├── .env.example
└── README.md
```

## Схема связей базы данных

```
ApplicationUser (1) ─── Identity таблицы

Team      (1) ──── (1) TeamStatistic
Team      (1) ──── (N) Match          [как HomeTeam]
Team      (1) ──── (N) Match          [как AwayTeam]
Match     (1) ──── (1) Prediction
Match     (N) ──── (N) SportsMarket   [через MarketOdds]
```

## Быстрый старт — локально

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Команды

```powershell
# 1. Восстановить пакеты NuGet
dotnet restore src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj

# 2. Запустить (БД создаётся автоматически)
dotnet run --project src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj
```

Приложение: **http://localhost:5000**

## Запуск через Docker Desktop

```powershell
# 1. Скопировать файл переменных окружения
copy .env.example .env

# 2. Собрать образ и запустить контейнер
docker compose up --build -d

# 3. Проверить работоспособность
curl http://localhost:8024/health
```

## EF Core миграции

```powershell
# Перейти в папку проекта
cd src/SportsAnalyticsWeb

# Создать начальную миграцию
dotnet ef migrations add InitialCreate

# Применить к базе данных
dotnet ef database update

# Просмотр списка миграций
dotnet ef migrations list
```

## Тесты

```powershell
dotnet test tests/SportsAnalytics.Tests/SportsAnalytics.Tests.csproj --verbosity normal
```

## Учётные данные по умолчанию

 Поле      Значение           

 Email     admin@sport.local  
 Пароль    Admin123!          

## Healthcheck

```
GET http://localhost:8024/health
```

## Публикация Docker-образа

```powershell
# Войти в Docker Hub
docker login

# Собрать с тегом Docker Hub
docker build -t dmitriyisarov/sports_analytics_course:latest .

# Отправить на Docker Hub
docker push dmitriyisarov/sports_analytics_course:latest
```