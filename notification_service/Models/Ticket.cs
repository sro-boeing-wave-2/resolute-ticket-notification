using System;
using System.Collections.Generic;
using System.Text;

namespace notification_service.Models
{
    class Ticket
    {
        public string TicketId { get; set; }
        public string Intent { get; set; }
        public string Description { get; set; }
        public string AgentEmailid { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UserEmailId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? Closedon { get; set; }
        public string Closedby { get; set; }
        public int? Feedbackscore { get; set; }
    }
}
