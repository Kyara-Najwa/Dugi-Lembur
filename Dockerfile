# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Salin file proyek dulu (agar cache build lebih efisien)
COPY *.csproj ./
RUN dotnet restore

# Salin semua file setelah restore selesai
COPY . .

# Publish ke folder /app/out
RUN dotnet publish -c Release -o /app/out

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Salin hasil publish dari build stage
COPY --from=build /app/out .

# Set environment variable agar ASP.NET berjalan di port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Jalankan aplikasi
ENTRYPOINT ["dotnet", "Mobile.dll"]

