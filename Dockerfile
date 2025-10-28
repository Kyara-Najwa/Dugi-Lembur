# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Salin semua file ke container
COPY . .

# Publish ke folder out
RUN dotnet publish -c Release -o out

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Salin hasil build dari stage pertama
COPY --from=build /app/out .

# Set environment variable agar ASP.NET jalan di port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Jalankan aplikasi
ENTRYPOINT ["dotnet", "Dugi-lembur.dll"]
