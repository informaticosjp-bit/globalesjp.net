# =========================================================================
# GLOBALES JP | Servicios TI & Software
# Production Dockerfile para Render & Entornos de Contenedores
# Target Framework: ASP.NET Core MVC (.NET 10.0)
# =========================================================================

# --- Fase 1: Base Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

# Puerto dinámico: Render inyecta la variable de entorno $PORT en tiempo de ejecución.
# Establecemos 8080 como valor predeterminado compatible con .NET 8/10.
ENV PORT=8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# --- Fase 2: Build SDK ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copiar primero el archivo de proyecto para optimizar la caché de capas de Docker
COPY ["GLOBALESJP.NET.csproj", "./"]
RUN dotnet restore "./GLOBALESJP.NET.csproj"

# 2. Copiar el código fuente completo del proyecto
COPY . .
RUN dotnet build "./GLOBALESJP.NET.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- Fase 3: Publish ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GLOBALESJP.NET.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- Fase 4: Imagen Final de Producción ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Entrypoint apuntando a la DLL final de la aplicación
ENTRYPOINT ["dotnet", "GLOBALESJP.NET.dll"]
