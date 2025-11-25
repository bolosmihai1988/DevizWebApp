# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiem toate fișierele din proiect
COPY . .

# Restaurăm dependențele
RUN dotnet restore

# Publicăm aplicația în folderul /app/out
RUN dotnet publish DevizWebApp/DevizWebApp.csproj -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiem aplicația publicată
COPY --from=build /app/out ./

# Expunem portul (Render îl va suprascrie cu PORT-ul său)
EXPOSE 10000

# Entry point
ENTRYPOINT ["dotnet", "DevizWebApp.dll"]
