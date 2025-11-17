# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiem fișierul csproj și restaurăm dependențele
COPY *.csproj ./
RUN dotnet restore

# Copiem restul codului
COPY . ./

# Publicăm proiectul direct (nu soluția)
RUN dotnet publish DevizWebApp.csproj -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80

# Numele DLL-ului trebuie să fie exact cel al proiectului tău
ENTRYPOINT ["dotnet", "DevizWebApp.dll"]
