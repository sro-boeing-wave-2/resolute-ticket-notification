using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using notification_service.Models;
using Newtonsoft.Json;
using System.Threading;

namespace notification_service.Hubs
{
    class NotificationHub : Hub
    {
        public static Dictionary<string, string> connectionMapping = new Dictionary<string, string>();

        public async Task SendNotification(string email, string message)
        {
            if (NotificationHub.connectionMapping.ContainsKey(email)) { 
                    Console.WriteLine("Sending Notification: " + message);
                    Console.WriteLine("ConnectionId " + NotificationHub.connectionMapping[email]);
                    if (!string.IsNullOrEmpty(NotificationHub.connectionMapping[email]))
                    {
                        Console.WriteLine("Found user, sending the message.");
                        await Clients.Client(NotificationHub.connectionMapping[email]).SendAsync("ReceiveNotification", message);
                    }
            } else
            {
                Console.WriteLine("Email not registered in the mapping.");
            }
        }

        public async Task Config(string email)
        {
            if (NotificationHub.connectionMapping.ContainsKey(email))
            {
                NotificationHub.connectionMapping[email] = Context.ConnectionId;
            }
            else
            {
                NotificationHub.connectionMapping.Add(email, Context.ConnectionId);
            }
            Console.WriteLine("ConnectionId: " + NotificationHub.connectionMapping[email] + " Email: " + email + " added to the mapping.");
        }
    }
}
