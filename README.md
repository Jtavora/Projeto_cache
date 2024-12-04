# Sistema de Microsserviços com Cache Dinâmico

---

## Como Executar o Projeto

1. **Suba os containers com Docker Compose:**
 ```bash
   docker compose up -d
```

2. **Aguarde o container de migração do Kong (kong-migrations) finalizar.**

3. **Inicie o container do Kong (kong) novamente, ele acaba caindo junto depois da migration.**

4. **Renicie o container consumer.**

5. **Execute o script para configurar o Kong automaticamente:**
  ```bash
   ./afterDocker.sh
  ```

6. **Acesse as APIs por meio do Kong.**
   - Utilize a collection do Postman disponível no repositório (Pasta Postman) para importar e testar as requisições.
   - O acess_token esta configurado para expirar em 2 minutos.

7. **Acesse o front-end para monitorar os logs em tempo real:**
   - URL do front: [http://localhost:5255/front.html](http://localhost:5255/front.html).

8. ***Permissions:***
   - logCheck --> Acesso ao microserviço de log.
   - eCommerce --> Acesso ao microserviço de e-commerce.
   - create_permission --> Feature para criar novas permissões.
   - delete_permission --> Feature para deletar permissões.
   - get_all_permissions --> Feature para listar todas as permissões.
   - create_user --> Feature para criar novos usuários.
   - update_user --> Feature para atualizar usuários.
   - get_all_users --> Feature para listar todos os usuários.
---

## Descrição
Este projeto consiste em um sistema de microsserviços projetado para atender a demandas de alta performance, como em e-commerces. A arquitetura utiliza cache dinâmico para otimizar o desempenho, um barramento de eventos para sincronizar atualizações entre serviços e práticas modernas de autenticação e monitoramento.

---

## Arquitetura de Microsserviços

### **AUTH**
- **Framework:** FastAPI
- **Banco de Dados:** MongoDB
- **Funcionalidades:**
  - Autenticação de toda a aplicação.
  - Implementação de RBAC e ABAC.
  - Uso de cache para validação de tokens.
- **Pontos Interessantes:**
  - Utilização do campo `iss` no payload do token para identificar o emissor.
  - Cache eficiente, sem necessidade de invalidação explícita do consumidor.

---

### **ECOMMERCE**
- **Framework:** .NET
- **Banco de Dados:** SQLServer
- **Funcionalidades:**
  - Gerenciamento de produtos e vendas.
  - API assíncrona para consultas e manipulação de dados.
  - Cache para dados específicos (produtos e vendas).
- **Pontos Interessantes:**
  - Estrutura assíncrona para evitar privação do pool de threads.
  - Uso extensivo de boas práticas .NET.

---

### **CONSUMER**
- **Framework:** Node.js
- **Funcionalidades:**
  - Declaração e consumo de filas RabbitMQ.
  - Invalidação do cache em caso de atualizações.
  - Monitora alterações em produtos e vendas.
- **Filas Monitoradas:**
  - `product_changes`
  - `sales_updates`
- **Pontos Interessantes:**
  - Gerenciamento eficiente de filas e comunicação entre serviços.

---

### **LOG-MONITORING**
- **Framework:** .NET com SignalR
- **Banco de Dados:** Redis
- **Funcionalidades:**
  - Centralização de logs dos microsserviços.
  - Monitoramento em tempo real via front-end.
  - Registro detalhado de endpoints:
    - AUTH: Todos os endpoints.
    - ECOMMERCE: Endpoints de `create`, `update` e `delete`.

---

### **KONG**
- **Funcionalidades:**
  - Integração e gerenciamento de APIs dos microsserviços.
  - Plugins personalizados para controle de acesso.
  - Bloqueio de serviços com base em `roles` e `features`.
- **Pontos Interessantes:**
  - Configuração automatizada via script.
  - Plugin `feature-check` para validação de permissões.

---

## Tecnologias Utilizadas
- **Linguagens e Frameworks:**
  - Python (FastAPI)
  - .NET Core
  - Node.js
- **Bancos de Dados:**
  - MongoDB
  - SQLServer
  - Redis
- **Mensageria:**
  - RabbitMQ
- **Gateway de API:**
  - Kong com plugins personalizados.

---

## Diagramas

### **Diagrama de Sequência**
![Diagrama de Sequência](out/AplicationDiagrams/sequencediagram/sequencediagram.png)

### **Diagrama de Classe**
![Diagrama de Classe](out/AplicationDiagrams/classdiagram/classdiagram.png)