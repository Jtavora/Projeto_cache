import pika
import redis
import json

# Configurações do Redis
redis_client = redis.StrictRedis(host='redis', port=6379, db=0)

# Configurações do RabbitMQ
rabbitmq_host = 'rabbitmq'
queue_name = 'product_changes'

def invalidate_cache(product_id):
    redis_client.delete(f'produto:{product_id}')
    print(f'Cache invalidado para o produto {product_id}')

def callback(ch, method, properties, body):
    message = body.decode()
    print(f"\n [x] Received {message}")
    try:
        data = json.loads(message)
        if 'Produto' in data:
            product_id = data.get('Produto')
            invalidate_cache(product_id)
    except json.JSONDecodeError:
        print("Erro ao decodificar a mensagem")

def main():
    print("Iniciando o consumidor...")
    try:
        credentials = pika.PlainCredentials('admin', 'teste')
        connection = pika.BlockingConnection(pika.ConnectionParameters(
            host=rabbitmq_host,
            credentials=credentials
        ))
        print("Conexão com RabbitMQ estabelecida com sucesso.")
        channel = connection.channel()

        channel.queue_declare(queue=queue_name, durable=True)
        print("Fila declarada.")

        channel.basic_consume(queue=queue_name, on_message_callback=callback, auto_ack=True)
    except Exception as e:
        print(f"Erro ao conectar ao RabbitMQ: {e}")
        return

    print(" [*] Esperando por mensagens. Para sair, pressione CTRL+C")
    channel.start_consuming()

if __name__ == '__main__':
    main()