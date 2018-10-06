﻿using Newtonsoft.Json;
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
    class RabbitMQListener
    {
        private HubConnection _connection;

        public RabbitMQListener()
        {
            _connection = new HubConnectionBuilder().WithUrl("http://" + Constants.BASE_URL + "/notifications").Build();
            _connection.StartAsync();
        }

        private NotificationHub notificationHub = new NotificationHub();

        public void startListener()
        {
            Console.WriteLine("Starting RabbitMQ listener");
            var factory = new ConnectionFactory() { HostName = Constants.BASE_URL };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "ticket-notification", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    var subscription = new Subscription(channel, "ticket-notification", false);
                    while (true)
                    {
                        BasicDeliverEventArgs basicDeliveryEventArgs =
                            subscription.Next();
                        Console.WriteLine("Received Message: " + JsonConvert.SerializeObject(basicDeliveryEventArgs));
                        string messageContent =
                            Encoding.UTF8.GetString(basicDeliveryEventArgs.Body);
                        Console.WriteLine(messageContent);
                        subscription.Ack(basicDeliveryEventArgs);
                        Console.WriteLine("Received Message");
                        Ticket ticket = JsonConvert.DeserializeObject<Ticket>(messageContent);
                        Notification notificationMessage = new Notification();
                        notificationMessage.ticketId = ticket.TicketId;
                        notificationMessage.title = "You have been assigned a ticket";
                        notificationMessage.description = ticket.Description;
                        notificationMessage.createdOn = DateTime.Now;
                        Console.WriteLine("Sending Notification");
                        _connection.InvokeAsync("SendNotification", ticket.AgentEmailid, notificationMessage);
                        Console.WriteLine(" [x] Sent {0}", JsonConvert.SerializeObject(notificationMessage));
                    }
                    //var consumer = new EventingBasicConsumer(channel);
                    //consumer.Received += (model, ea) =>
                    //                {
                    //                    Console.WriteLine("Received Message");
                    //                    var body = ea.Body;
                    //                    var message = Encoding.UTF8.GetString(body);
                    //                    Ticket ticket = JsonConvert.DeserializeObject<Ticket>(message);
                    //                    Notification notificationMessage = new Notification();
                    //                    notificationMessage.ticketId = ticket.TicketId;
                    //                    notificationMessage.title = "You have been assigned a ticket";
                    //                    notificationMessage.description = ticket.Description;
                    //                    notificationMessage.createdOn = DateTime.Now;
                    //                    Console.WriteLine("Sending Notification");
                    //                    _connection.InvokeAsync("SendNotification", ticket.AgentEmailid, notificationMessage);
                    //                    Console.WriteLine(" [x] Sent {0}", JsonConvert.SerializeObject(notificationMessage));
                    //                };
                    //channel.BasicConsume(queue: "ticket-notification", autoAck: true, consumer: consumer);
                    //Console.ReadLine();
                }
            }

        }
    }
}
