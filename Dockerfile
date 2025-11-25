# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiem fișierele de proiect
COPY *.sln ./
COPY DevizWebApp/*.csproj ./DevizWebApp/

# Restaurăm dependențele
RUN dotnet restore

# Copiem codul sursă
COPY DevizWebApp/. ./DevizWebApp/

# Publicăm aplicația
RUN dotnet publish DevizWebApp/DevizWebApp.csproj -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiem aplicația publicată
COPY --from=build /app/out ./

# Expunem portul Render
EXPOSE 10000

# Setăm entrypoint
ENTRYPOINT ["dotnet", "DevizWebApp.dll"]
