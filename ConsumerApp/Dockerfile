# Use uma imagem base do Python
FROM python:3.8

# Crie um diretório de trabalho
WORKDIR /app

# Copie todos os arquivos para o contêiner
COPY . .

# Instale as dependências do Python
RUN pip install --no-cache-dir -r requirements.txt

# Conceder permissão de execução para o script
RUN chmod +x script.sh

# Defina o script como o ponto de entrada
CMD ["./script.sh"]