# Usando a imagem base do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Definir diretório de trabalho
WORKDIR /app

# Copiar todos os arquivos da aplicação para o container
COPY . ./

# Restaurar dependências
RUN dotnet restore

# Expor a porta para o container (ajuste conforme necessário)
EXPOSE 5255

# Rodar a aplicação com `dotnet run`
CMD ["dotnet", "run", "--urls", "http://0.0.0.0:5255"]