# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/WeatherApp.csproj", "src/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]

RUN dotnet restore "src/WeatherApp.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src"
RUN dotnet build "WeatherApp.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN find . -type d -name "obj" -exec rm -rf {} + || true
RUN dotnet publish "WeatherApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherApp.dll"]
