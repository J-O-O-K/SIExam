FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Services/CatalogService/CatalogService.API/CatalogService.API.csproj", "src/Services/CatalogService/CatalogService.API/"]
COPY ["src/Services/CatalogService/CatalogService.Application/CatalogService.Application.csproj", "src/Services/CatalogService/CatalogService.Application/"]
COPY ["src/Services/CatalogService/CatalogService.Domain/CatalogService.Domain.csproj", "src/Services/CatalogService/CatalogService.Domain/"]
COPY ["src/Services/CatalogService/CatalogService.Infrastructure/CatalogService.Infrastructure.csproj", "src/Services/CatalogService/CatalogService.Infrastructure/"]
COPY ["src/Services/CatalogService/CatalogService.Persistence/CatalogService.Persistence.csproj", "src/Services/CatalogService/CatalogService.Persistence/"]

RUN dotnet restore "src/Services/CatalogService/CatalogService.API/CatalogService.API.csproj"

# Copy all source files
COPY . .

WORKDIR "/src/src/Services/CatalogService/CatalogService.API"
RUN dotnet build "CatalogService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CatalogService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.API.dll"]