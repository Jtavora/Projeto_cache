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
const queueName = 'product_changes';

function invalidateCache(productId) {
    redisClient.del(`produto:${productId}`, (err, response) => {
      if (err) {
        console.error("Erro ao invalidar o cache:", err);
      } else {
        console.log(`Cache invalidado para o produto ${productId}`);
      }
    });
    console.log(`***Operação de cache concluída para o produto ${productId}***`);
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

    channel.assertQueue(queueName, { durable: true });
    console.log("Fila declarada.");

    channel.consume(queueName, (msg) => {
      const message = msg.content.toString();
      console.log(`\n [x] Received ${message}`);

      try {
        const data = JSON.parse(message);
        if (data.Produto) {
          invalidateCache(data.Produto);
        }
      } catch (error) {
        console.error("Erro ao decodificar a mensagem:", error);
      }
    }, { noAck: true });

    console.log(" [*] Esperando por mensagens. Para sair, pressione CTRL+C");
  });
});