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

# Execute o aplicativo
CMD ["node", "consumer.js"]