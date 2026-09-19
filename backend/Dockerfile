FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SistemaDeControle.sln ./
COPY src/SistemaDeControle.Api/SistemaDeControle.Api.csproj src/SistemaDeControle.Api/
COPY src/SistemaDeControle.Application/SistemaDeControle.Application.csproj src/SistemaDeControle.Application/
COPY src/SistemaDeControle.Domain/SistemaDeControle.Domain.csproj src/SistemaDeControle.Domain/
COPY src/SistemaDeControle.Infrastructure/SistemaDeControle.Infrastructure.csproj src/SistemaDeControle.Infrastructure/
COPY tests/SistemaDeControle.UnitTests/SistemaDeControle.UnitTests.csproj tests/SistemaDeControle.UnitTests/
COPY tests/SistemaDeControle.IntegrationTests/SistemaDeControle.IntegrationTests.csproj tests/SistemaDeControle.IntegrationTests/

RUN dotnet restore src/SistemaDeControle.Api/SistemaDeControle.Api.csproj

COPY . .

RUN dotnet publish src/SistemaDeControle.Api/SistemaDeControle.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SistemaDeControle.Api.dll"]
