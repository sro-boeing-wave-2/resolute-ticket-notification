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
        public Dictionary<string, string> connectionMapping = new Dictionary<string, string>();

        public async Task SendNotification(string email, string message)
        {
            if (connectionMapping.ContainsKey(email)) { 
                    Console.WriteLine("Sending Notification: " + message);
                    Console.WriteLine("ConnectionId " + connectionMapping[email]);
                    if (!string.IsNullOrEmpty(connectionMapping[email]))
                    {
                        Console.WriteLine("Found user, sending the message.");
                        await Clients.Client(connectionMapping[email]).SendAsync("ReceiveNotification", message);
                    }
            } else
            {
                Console.WriteLine("Email not registered in the mapping.");
            }
        }

        public async Task Config(string email)
        {
            if (connectionMapping.ContainsKey(email))
            {
                connectionMapping[email] = Context.ConnectionId;
            }
            else
            {
                connectionMapping.Add(email, Context.ConnectionId);
            }
            Console.WriteLine("ConnectionId: " + connectionMapping[email] + " Email: " + email + " added to the mapping.");
        }
    }
}
