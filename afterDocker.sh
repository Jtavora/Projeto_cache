#!/bin/bash

# URL da API Admin do Kong
KONG_ADMIN_URL="http://localhost:8001"

# Função para criar um serviço
create_service() {
    local service_name=$1
    local service_url=$2

    curl -i -X POST $KONG_ADMIN_URL/services \
        --data "name=$service_name" \
        --data "url=$service_url"
}

# Função para criar uma rota
create_route() {
    local service_name=$1
    local path=$2

    curl -i -X POST $KONG_ADMIN_URL/services/$service_name/routes \
        --data "paths[]=$path"
}

# Função para adicionar o plugin JWT
add_jwt_plugin() {
    local service_name=$1

    curl -i -X POST $KONG_ADMIN_URL/services/$service_name/plugins \
        --data "name=jwt"
}

# Função para criar um consumidor
create_consumer() {
    local consumer_name=$1

    curl -i -X POST $KONG_ADMIN_URL/consumers \
        --data "username=$consumer_name"
}

# Função para adicionar credenciais JWT a um consumidor
add_jwt_credential() {
    local consumer_name=$1
    local key=$2
    local secret=$3

    curl -i -X POST $KONG_ADMIN_URL/consumers/$consumer_name/jwt \
        --data "key=$key" \
        --data "secret=$secret"
}

# Criar serviços
create_service "ecommerce-service" "http://ecommerce:5171"
create_service "log-monitoring-service" "http://log-monitoring:5255"

# Criar rotas
create_route "ecommerce-service" "/api/ecommerce"
create_route "log-monitoring-service" "/api/log-monitoring"

# Adicionar plugin JWT para proteção
add_jwt_plugin "ecommerce-service"
add_jwt_plugin "log-monitoring-service"

# Criar consumidores
create_consumer "ecommerce-consumer"
create_consumer "log-monitoring-consumer"

# Adicionar credenciais JWT
add_jwt_credential "ecommerce-consumer" "auth-service" "5c7f45d6f733fc57ea8a3cea62a82531eb93b35987853d93d3d79d32329e4278"
add_jwt_credential "log-monitoring-consumer" "auth-service" "5c7f45d6f733fc57ea8a3cea62a82531eb93b35987853d93d3d79d32329e4278"

echo "Configuração completa!"