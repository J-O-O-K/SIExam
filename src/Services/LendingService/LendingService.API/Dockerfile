FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/LendingService/LendingService.API/LendingService.API.csproj", "src/Services/LendingService/LendingService.API/"]
COPY ["src/Services/LendingService/LendingService.Application/LendingService.Application.csproj", "src/Services/LendingService/LendingService.Application/"]
COPY ["src/Services/LendingService/LendingService.Domain/LendingService.Domain.csproj", "src/Services/LendingService/LendingService.Domain/"]
COPY ["src/Services/LendingService/LendingService.Infrastructure/LendingService.Infrastructure.csproj", "src/Services/LendingService/LendingService.Infrastructure/"]
COPY ["src/Services/LendingService/LendingService.Persistence/LendingService.Persistence.csproj", "src/Services/LendingService/LendingService.Persistence/"]

RUN dotnet restore "src/Services/LendingService/LendingService.API/LendingService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/LendingService/LendingService.API"
RUN dotnet build "LendingService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LendingService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LendingService.API.dll"]