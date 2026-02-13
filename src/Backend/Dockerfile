# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files
COPY MultiServices.Domain/MultiServices.Domain.csproj MultiServices.Domain/
COPY MultiServices.Application/MultiServices.Application.csproj MultiServices.Application/
COPY MultiServices.Infrastructure/MultiServices.Infrastructure.csproj MultiServices.Infrastructure/
COPY MultiServices.API/MultiServices.API.csproj MultiServices.API/

# Restore
RUN dotnet restore MultiServices.API/MultiServices.API.csproj

# Copy everything
COPY . .

# Build
RUN dotnet build MultiServices.API/MultiServices.API.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish MultiServices.API/MultiServices.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5000

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MultiServices.API.dll"]
