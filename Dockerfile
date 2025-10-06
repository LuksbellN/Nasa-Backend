# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["src/Nasa.API/Nasa.API.csproj", "src/Nasa.API/"]
COPY ["src/Nasa.Domain/Nasa.Domain.csproj", "src/Nasa.Domain/"]
COPY ["src/Nasa.Data/Nasa.Data.csproj", "src/Nasa.Data/"]
COPY ["src/Nasa.IoC/Nasa.IoC.csproj", "src/Nasa.IoC/"]
COPY ["src/Nasa.Resources/Nasa.Resources.csproj", "src/Nasa.Resources/"]

RUN dotnet restore "src/Nasa.API/Nasa.API.csproj"

# Copiar todo o código e fazer o build
COPY . .
WORKDIR "/src/src/Nasa.API"
RUN dotnet build "Nasa.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "Nasa.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copiar arquivos publicados
COPY --from=publish /app/publish .

# Definir variáveis de ambiente para produção
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Nasa.API.dll"]

