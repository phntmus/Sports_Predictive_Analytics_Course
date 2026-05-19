# EF Core migrations

Для разработки миграции создаются командой:

```powershell
dotnet ef migrations add InitialCreate --project src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj
dotnet ef database update --project src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj
```

В Docker-версии приложение при запуске проверяет наличие миграций. Если миграции есть, применяется `Database.MigrateAsync()`. Если проект запускается как демонстрационный без сгенерированных миграций, используется `EnsureCreatedAsync()`, чтобы база была создана автоматически.
