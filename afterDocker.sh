#!/bin/bash

# URL da API Admin do Kong
KONG_ADMIN_URL="http://localhost:8001"

# Definição de cores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # Sem cor

# Função para criar um serviço
create_service() {
    local service_name=$1
    local service_url=$2

    echo -e "${BLUE}\nCriando serviço: $service_name${NC}"
    curl -i -X POST $KONG_ADMIN_URL/services \
        --data "name=$service_name" \
        --data "url=$service_url"
}

# Função para criar uma rota
create_route() {
    local service_name=$1
    local path=$2

    echo -e "${BLUE}\nCriando rota para serviço: $service_name com path: $path${NC}"
    curl -i -X POST $KONG_ADMIN_URL/services/$service_name/routes \
        --data "paths[]=$path"
}

# Função para adicionar o plugin JWT
add_jwt_plugin() {
    local service_name=$1

    echo -e "${YELLOW}\nAdicionando plugin JWT para serviço: $service_name${NC}"
    curl -i -X POST $KONG_ADMIN_URL/services/$service_name/plugins \
        --data "name=jwt" \
        --data "config.claims_to_verify=exp"
}

# Função para criar um consumidor
create_consumer() {
    local consumer_name=$1

    echo -e "${BLUE}\nCriando consumidor: $consumer_name${NC}"
    curl -i -X POST $KONG_ADMIN_URL/consumers \
        --data "username=$consumer_name"
}

# Função para adicionar credenciais JWT a um consumidor
add_jwt_credential() {
    local consumer_name=$1
    local key=$2
    local secret=$3

    # Verificar se a credencial já existe
    if curl -s $KONG_ADMIN_URL/consumers/$consumer_name/jwt | grep -q "\"key\":\"$key\""; then
        echo -e "${GREEN}Credencial JWT já existe para $consumer_name com key $key${NC}"
    else
        echo -e "${YELLOW}Adicionando credencial JWT para $consumer_name com key $key${NC}"
        curl -i -X POST $KONG_ADMIN_URL/consumers/$consumer_name/jwt \
            --data "key=$key" \
            --data "secret=$secret"
    fi
}

# Criar serviços
create_service "ecommerce-service" "http://ecommerce:5171"
create_service "log-monitoring-service" "http://log-monitoring:5255"
create_service "auth-service" "http://auth:4000"

# Criar rotas
create_route "ecommerce-service" "/ecommerce"
create_route "log-monitoring-service" "/log-monitoring"
create_route "auth-service" "/auth"

# Adicionar plugin JWT para proteção
add_jwt_plugin "ecommerce-service"
add_jwt_plugin "log-monitoring-service"

# Criar um consumidor compartilhado
create_consumer "shared-consumer"

# Adicionar credenciais JWT ao consumidor compartilhado
add_jwt_credential "shared-consumer" "auth" "5c7f45d6f733fc57ea8a3cea62a82531eb93b35987853d93d3d79d32329e4278"

echo -e "${GREEN}\nConfiguração completa!${NC}"