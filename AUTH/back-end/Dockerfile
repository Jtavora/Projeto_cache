# Use uma imagem base oficial do Python
FROM python:latest

# Defina o diretório de trabalho dentro do contêiner
WORKDIR /app

# Copie os arquivos de requisitos para o contêiner
COPY requirements.txt requirements.txt

# Instale as dependências do Python
RUN pip install -r requirements.txt

# Copie o restante do código do projeto para o contêiner
COPY . .

# Exponha a porta que a aplicação vai rodar
EXPOSE 4000

# Comando para rodar a aplicação e executar migrações Alembic
CMD ["sh", "-c", "uvicorn main:app --host 0.0.0.0 --port 4000 --reload"]