const amqp = require('amqplib/callback_api');
const redis = require('redis');

// Configurações do Redis
const redisClient = redis.createClient({
  url: 'redis://redis:6379'
});

redisClient.on('error', (err) => {
  console.error('Erro de conexão com o Redis:', err);
});

redisClient.on('connect', () => {
  console.log('Conexão com Redis estabelecida com sucesso.');
});

// Conecte ao Redis
redisClient.connect().catch(err => {
  console.error('Erro ao conectar ao Redis:', err);
});

// Configurações do RabbitMQ
const rabbitmqHost = 'amqp://admin:teste@rabbitmq';
const productQueueName = 'product_changes';
const salesQueueName = 'sales_updates';

function invalidateProductCache(productId) {
  redisClient.del(`produto:${productId}`, (err, response) => {
    if (err) {
      console.error("Erro ao invalidar o cache do produto:", err);
    } else {
      console.log(`Cache invalidado para o produto ${productId}`);
    }
  });
  console.log(`***Operação de cache concluída para o produto ${productId}***`);
}

function invalidateSaleCache(saleId) {
  redisClient.del(`venda:${saleId}`, (err, response) => {
    if (err) {
      console.error("Erro ao invalidar o cache da venda:", err);
    } else {
      console.log(`Cache invalidado para a venda ${saleId}`);
    }
  });
  console.log(`***Operação de cache concluída para a venda ${saleId}***`);
}

amqp.connect(rabbitmqHost, (error0, connection) => {
  if (error0) {
    console.error("Erro ao conectar ao RabbitMQ:", error0);
    return;
  }
  console.log("Conexão com RabbitMQ estabelecida com sucesso.");
  
  connection.createChannel((error1, channel) => {
    if (error1) {
      console.error("Erro ao criar canal:", error1);
      return;
    }

    // Configurar fila de produtos
    channel.assertQueue(productQueueName, { durable: true });
    console.log("Fila de produtos declarada.");

    channel.consume(productQueueName, (msg) => {
      const message = msg.content.toString();
      console.log(`\n [x] Produto recebido: ${message}`);

      try {
        const data = JSON.parse(message);
        if (data.Produto) {
          invalidateProductCache(data.Produto);
        }
      } catch (error) {
        console.error("Erro ao decodificar a mensagem do produto:", error);
      }
    }, { noAck: false });

    // Configurar fila de vendas
    channel.assertQueue(salesQueueName, { durable: true });
    console.log("Fila de vendas declarada.");

    channel.consume(salesQueueName, (msg) => {
      const message = msg.content.toString();
      console.log(`\n [x] Venda recebida: ${message}`);

      try {
        const data = JSON.parse(message);
        if (data.Venda) {
          invalidateSaleCache(data.Venda);
        }
      } catch (error) {
        console.error("Erro ao decodificar a mensagem de venda:", error);
      }
    }, { noAck: false });

    console.log(" [*] Esperando por mensagens. Para sair, pressione CTRL+C");
  });
});