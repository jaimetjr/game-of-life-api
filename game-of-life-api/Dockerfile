# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the API csproj and restore
COPY game-of-life-api.csproj ./
RUN dotnet restore

# Copy the rest of the API source and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose HTTP and configure Kestrel
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Ensure the data folder exists for SQLite persistence
RUN mkdir -p /app/App_Data

ENTRYPOINT ["dotnet", "game-of-life-api.dll"]