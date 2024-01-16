using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWIthCQRS.Infrastructure.Helper.RabbitMQHelper
{
    public class RabbitMQHelper
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchange;
        private readonly string _queue;
        private readonly string _routingKey;

        public RabbitMQHelper(string host, string exchange, string queue, string routingKey)
        {
            var factory = new ConnectionFactory() { HostName = host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchange = exchange;
            _queue = queue;
            _routingKey = routingKey;

            // Declare exchange and queue
            _channel.ExchangeDeclare(_exchange, ExchangeType.Direct);
            _channel.QueueDeclare(_queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(_queue, _exchange, _routingKey);
        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: _exchange, routingKey: _routingKey, basicProperties: null, body: body);
        }

        public void ConsumeMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                // Handle the received message as needed
                Console.WriteLine($"Received message: {message}");
            };

            _channel.BasicConsume(queue: _queue, autoAck: true, consumer: consumer);
        }

        public void CloseConnection()
        {
            _connection.Close();
        }
    }
}
