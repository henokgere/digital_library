# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore as a separate, cacheable layer
COPY digital_library.csproj ./
RUN dotnet restore digital_library.csproj

# Copy the rest of the source and publish a Release build
COPY . .
RUN dotnet publish digital_library.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production

# Documentation only — Render injects the real port via the PORT env var.
EXPOSE 8080

# Render terminates TLS at its proxy and sets PORT at runtime.
# Bind Kestrel to that port (falls back to 8080 for local `docker run`).
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet digital_library.dll"]
