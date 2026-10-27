# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore inputs must be present before restore because every project imports
# the repository's common targets and centrally managed package versions.
COPY ["Directory.Packages.props", "."]
COPY ["common.targets", "."]
COPY ["sources/back/HeroService.Consumer/HeroService.Consumer.csproj", "sources/back/HeroService.Consumer/"]
COPY ["sources/back/HeroService.Application/HeroService.Application.csproj", "sources/back/HeroService.Application/"]
COPY ["sources/back/HeroService.Contracts/HeroService.Contracts.csproj", "sources/back/HeroService.Contracts/"]
COPY ["sources/back/HeroService.Domain/HeroService.Domain.csproj", "sources/back/HeroService.Domain/"]
COPY ["sources/back/HeroService.Persistence/HeroService.Persistence.csproj", "sources/back/HeroService.Persistence/"]

RUN dotnet restore "sources/back/HeroService.Consumer/HeroService.Consumer.csproj" \
    --source https://api.nuget.org/v3/index.json

COPY . .
WORKDIR "/src/sources/back/HeroService.Consumer"
RUN dotnet build "HeroService.Consumer.csproj" -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "HeroService.Consumer.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN useradd -m appuser
USER appuser

ENTRYPOINT ["dotnet", "HeroService.Consumer.dll"]