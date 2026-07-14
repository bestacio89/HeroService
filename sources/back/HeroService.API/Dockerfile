# syntax=docker/dockerfile:1

# ---------------------------------------------------------
# Base runtime image
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# ---------------------------------------------------------
# Build stage
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy nuget config and project files first (Docker layer caching:
# restore only re-runs if a .csproj or nuget.config actually changed)
COPY ["nuget.config", "."]
COPY ["sources/back/HeroService.API/HeroService.API.csproj", "sources/back/HeroService.API/"]
COPY ["sources/back/HeroService.Application/HeroService.Application.csproj", "sources/back/HeroService.Application/"]
COPY ["sources/back/HeroService.Contracts/HeroService.Contracts.csproj", "sources/back/HeroService.Contracts/"]
COPY ["sources/back/HeroService.Domain/HeroService.Domain.csproj", "sources/back/HeroService.Domain/"]
COPY ["sources/back/HeroService.Persistence/HeroService.Persistence.csproj", "sources/back/HeroService.Persistence/"]

RUN dotnet restore "sources/back/HeroService.API/HeroService.API.csproj"

# Copy the rest of the source and build
COPY . .
WORKDIR "/src/sources/back/HeroService.API"
RUN dotnet build "HeroService.API.csproj" -c Release -o /app/build

# ---------------------------------------------------------
# Publish stage
# ---------------------------------------------------------
FROM build AS publish
RUN dotnet publish "HeroService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------------------------------------------------------
# Final runtime image
# ---------------------------------------------------------
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN useradd -m appuser
USER appuser

ENTRYPOINT ["dotnet", "HeroService.API.dll"]