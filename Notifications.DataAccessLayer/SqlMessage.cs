using System;

namespace Notifications.DataAccessLayer
{
    public class SqlMessage
    {
        public SqlMessage()
        {
            Date = DateTime.Now;
            Sender = new SqlEmployee();
            Receiver = new SqlEmployee();
        }

        public int MessageId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public SqlEmployee Sender { get; set; }
        public int SenderId { get; set; }

        public SqlEmployee Receiver { get; set; }
        public int ReceiverId { get; set; }
    }
}