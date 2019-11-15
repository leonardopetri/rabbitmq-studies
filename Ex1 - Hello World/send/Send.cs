﻿using System;
using RabbitMQ.Client;
using System.Text;

namespace send
{
    class Send
    {
        static void Main(string[] args)
        {
            var resend = string.Empty;
            do
            {
                var factory = new ConnectionFactory() { HostName = "192.168.99.100" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "hello",
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                        string message = "Hello World!";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                                routingKey: "hello",
                                                basicProperties: null,
                                                body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.WriteLine(" Press [r] to re-send message.");
                resend = Console.ReadLine();

            } while (resend.Equals("r"));
        }
    }
}
