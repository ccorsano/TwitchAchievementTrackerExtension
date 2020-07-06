#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
# Install cultures (same approach as Alpine SDK image)
RUN apk add --no-cache icu-libs
# Disable the invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TwitchAchievementTrackerBackend/TwitchAchievementTrackerBackend.csproj", "TwitchAchievementTrackerBackend/"]
RUN dotnet restore "TwitchAchievementTrackerBackend/TwitchAchievementTrackerBackend.csproj"
COPY . .
WORKDIR "/src/TwitchAchievementTrackerBackend"
RUN dotnet build "TwitchAchievementTrackerBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitchAchievementTrackerBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitchAchievementTrackerBackend.dll"]