using System;
using RabbitMQ.Client;
using System.Text;
using System.Linq;

namespace EmitLog
{
    class EmitLog
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "192.168.99.100" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "topic_log", type: "topic");

                    var routingKey = GetRoutingKey(args);
                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "topic_log", routingKey: routingKey, basicProperties: null, body: body);

                    Console.WriteLine(" [x] Sent '{0}': '{1}'", routingKey, message);
                }
            }
        }

        private static string GetRoutingKey(string[] args)
        {
            return ((args.Length > 0) ? args[0] : "anonymous.info");
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!");
        }
    }
}
