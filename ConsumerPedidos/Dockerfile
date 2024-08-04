# Use a imagem oficial do .NET Core SDK como base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copie o arquivo .csproj e restaure dependências
COPY *.csproj ./
RUN dotnet restore

# Copie todo o código e compile o projeto
COPY . ./
RUN dotnet publish -c Release -o out

# Use a imagem oficial do .NET Core runtime como base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Defina o comando de entrada para o contêiner
ENTRYPOINT ["dotnet", "ConsumerPedidos.dll"]
