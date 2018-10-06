using System;
using System.Collections.Generic;
using System.Text;

namespace notification_service.Models
{
    class Notification
    {
        public string ticketId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime createdOn { get; set; }
    }
}
