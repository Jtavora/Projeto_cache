using RabbitMQ.Client;
using System.Text;
using E_Commerce.Interfaces;

namespace E_Commerce.Services
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IConnection _conn;

        public RabbitMQPublisher(IConnection connection)
        {
            _conn = connection;
        }

        public async Task PublishAsync(string message)
        {
            using var channel = await _conn.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "product_changes", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(exchange: "", routingKey: "product_changes", body: body);
        }
    }
}