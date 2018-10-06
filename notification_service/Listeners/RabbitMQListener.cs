using Newtonsoft.Json;
using notification_service.Hubs;
using notification_service.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Text;
using notification_service.Utils;
using RabbitMQ.Client.MessagePatterns;

namespace notification_service.Listeners
{
    public class RabbitMQListener : IListener
    {
        private HubConnection _connection;

        public RabbitMQListener()
        {
        }

        public async void startListener()
        {
            Console.WriteLine("Starting RabbitMQ listener");
            var factory = new ConnectionFactory() { HostName = "13.126.8.255" };//Constants.BASE_URL };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "ticket-notification", durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
                            {
                                Console.WriteLine("Received Message");
                                var body = ea.Body;
                                var message = Encoding.UTF8.GetString(body);
                                Ticket ticket = JsonConvert.DeserializeObject<Ticket>(message);
                                Notification notificationMessage = new Notification();
                                notificationMessage.ticketId = ticket.TicketId;
                                notificationMessage.title = "You have been assigned a ticket";
                                notificationMessage.description = ticket.Description;
                                notificationMessage.createdOn = DateTime.Now;
                                Console.WriteLine("Sending Notification");
                                _connection = new HubConnectionBuilder().WithUrl("http://13.126.8.255/notification/notifications").Build();
                                _connection.StartAsync();
                                _connection.InvokeAsync("SendNotification", ticket.AgentEmailid, JsonConvert.SerializeObject(notificationMessage));
                                Console.WriteLine(" [x] Sent {0}", JsonConvert.SerializeObject(notificationMessage));
                            };
            channel.BasicConsume(queue: "ticket-notification", autoAck: true, consumer: consumer);

        }
    }
}
