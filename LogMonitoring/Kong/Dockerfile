# Usa a imagem oficial do Kong como base
FROM kong:latest

# Alterna temporariamente para o usuário root para instalar o netcat
USER root

# Instala o netcat (nc) para que o wait-for-it.sh funcione corretamente
RUN apt-get update && apt-get install -y netcat

# Retorna ao usuário padrão do Kong
USER kong