using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace ReceiveLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "192.168.99.100" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "topic_log", type: "topic");

                    var queueName = channel.QueueDeclare().QueueName;

                    if (args.Length == 0)
                    {
                        Console.Error.WriteLine(" Usage: {0} [binding_key...]", Environment.GetCommandLineArgs()[0]);

                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                        Environment.ExitCode = 1;
                        return;
                    }

                    foreach (var bindingKey in args)
                    {
                        channel.QueueBind(queue: queueName, exchange: "topic_log", routingKey: bindingKey);
                    }

                    Console.WriteLine(" [*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine(" [x] '{0}': '{1}'", routingKey, message);
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
