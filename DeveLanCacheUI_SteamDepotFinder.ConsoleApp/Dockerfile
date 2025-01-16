#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DeveLanCacheUI_SteamDepotFinder.ConsoleApp/DeveLanCacheUI_SteamDepotFinder.ConsoleApp.csproj", "DeveLanCacheUI_SteamDepotFinder.ConsoleApp/"]
COPY ["DeveLanCacheUI_SteamDepotFinder/DeveLanCacheUI_SteamDepotFinder.csproj", "DeveLanCacheUI_SteamDepotFinder/"]
RUN dotnet restore "DeveLanCacheUI_SteamDepotFinder.ConsoleApp/DeveLanCacheUI_SteamDepotFinder.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/DeveLanCacheUI_SteamDepotFinder.ConsoleApp"
RUN dotnet build "DeveLanCacheUI_SteamDepotFinder.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_VERSION
ARG VER=${BUILD_VERSION:-1.0.0}
RUN dotnet publish "DeveLanCacheUI_SteamDepotFinder.ConsoleApp.csproj" -c Release -o /app/publish /p:Version=$VER /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveLanCacheUI_SteamDepotFinder.ConsoleApp.dll"]