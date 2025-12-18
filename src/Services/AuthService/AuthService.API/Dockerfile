FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/AuthService/AuthService.API/AuthService.API.csproj", "src/Services/AuthService/AuthService.API/"]
COPY ["src/Services/AuthService/AuthService.Domain/AuthService.Domain.csproj", "src/Services/AuthService/AuthService.Domain/"]
COPY ["src/Services/AuthService/AuthService.Persistence/AuthService.Persistence.csproj", "src/Services/AuthService/AuthService.Persistence/"]

RUN dotnet restore "src/Services/AuthService/AuthService.API/AuthService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/AuthService/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]