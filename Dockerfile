FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj src/SportsAnalyticsWeb/
RUN dotnet restore src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj

COPY . .
RUN dotnet publish src/SportsAnalyticsWeb/SportsAnalyticsWeb.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends wget && rm -rf /var/lib/apt/lists/*
RUN mkdir -p /data
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SportsAnalyticsWeb.dll"]
