# syntax=docker/dockerfile:1

# ---------------------------------------------------------
# Base runtime image
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# ---------------------------------------------------------
# Build stage
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["nuget.config", "."]
COPY ["sources/back/HeroService.Consumer/HeroService.Consumer.csproj", "sources/back/HeroService.Consumer/"]
COPY ["sources/back/HeroService.Application/HeroService.Application.csproj", "sources/back/HeroService.Application/"]
COPY ["sources/back/HeroService.Contracts/HeroService.Contracts.csproj", "sources/back/HeroService.Contracts/"]
COPY ["sources/back/HeroService.Domain/HeroService.Domain.csproj", "sources/back/HeroService.Domain/"]
COPY ["sources/back/HeroService.Persistence/HeroService.Persistence.csproj", "sources/back/HeroService.Persistence/"]

RUN dotnet restore "sources/back/HeroService.Consumer/HeroService.Consumer.csproj"

COPY . .
WORKDIR "/src/sources/back/HeroService.Consumer"
RUN dotnet build "HeroService.Consumer.csproj" -c Release -o /app/build

# ---------------------------------------------------------
# Publish stage
# ---------------------------------------------------------
FROM build AS publish
RUN dotnet publish "HeroService.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------------------------------------------------------
# Final runtime image
# ---------------------------------------------------------
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN useradd -m appuser
USER appuser

ENTRYPOINT ["dotnet", "HeroService.Consumer.dll"]