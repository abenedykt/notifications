using System;

namespace Notifications.DataAccessLayer.SqlClass
{
    public class SqlMessage
    {
        public string MessageId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public SqlEmployee Sender { get; set; }
        public string SenderId { get; set; }

        public SqlEmployee Receiver { get; set; }
        public string ReceiverId { get; set; }
    }
}