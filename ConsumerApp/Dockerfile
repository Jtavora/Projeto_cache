# Use a imagem base do Node.js
FROM node:16

# Crie um diretório de trabalho
WORKDIR /app

# Copie o package.json e package-lock.json
COPY package*.json ./

# Instale as dependências
RUN npm install

# Copie o código da aplicação
COPY . .

# Copie o script wait-for-it
COPY wait-for-it.sh .
RUN chmod +x wait-for-it.sh

# Execute o aplicativo usando wait-for-it para o RabbitMQ
CMD ["./wait-for-it.sh", "rabbitmq:5672", "--", "node", "consumer.js"]