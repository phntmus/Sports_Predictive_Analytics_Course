# ── Этап 1: сборка ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Сначала копируем только .csproj для кеширования restore-слоя
COPY src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj src/SportsAnalyticsWeb/
RUN dotnet restore src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj

# Копируем весь исходный код
COPY . .

# Публикуем БЕЗ --no-restore, чтобы все зависимости были доступны
RUN dotnet publish src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ── Этап 2: runtime ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# wget нужен для healthcheck
RUN apt-get update && apt-get install -y --no-install-recommends wget \
    && rm -rf /var/lib/apt/lists/*

# Папка для SQLite-файла (монтируется как Docker volume)
RUN mkdir -p /data

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SportsAnalyticsWeb.dll"]