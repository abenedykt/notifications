using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public bool IfLogin { get; set; }

        public IList<Notification> SendNotifications { get; set; }

        public IList<Message> SendMessages { get; set; }

        public IList<Message> ReceiveMessages { get; set; }

        public int ReceiveNotificationId { get; set; }
    }


    public class ReceiverOfNotification
    {
        public int ReceiverId { get; set; }
        public int ReceiveNotificationId { get; set; }

        public Employee Receiver { get; set; }
        public Notification ReceiveNotification { get; set; }
    }


    public class Notification
    {
        
        public int NotificationId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public Employee Sender { get; set; }
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }
    }

    public class Message
    {
        public int MessageId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public Employee Sender { get; set; }
        public int SenderId { get; set; }

        public Employee Receiver { get; set; }
        public int ReceiverId { get; set; }
    }

    public class ContextNotifications : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ReceiverOfNotification> ReceiversOfNotification { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasKey(x => x.EmployeeId);
            modelBuilder.Entity<Notification>().HasKey(x => x.NotificationId);
            modelBuilder.Entity<Message>().HasKey(x => x.MessageId);

            modelBuilder.Entity<Notification>().HasRequired(x => x.Sender).WithMany(x => x.SendNotifications).HasForeignKey(x => x.SenderId);
            modelBuilder.Entity<Message>().HasRequired(x => x.Sender).WithMany(x => x.SendMessages).HasForeignKey(x => x.SenderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>().HasRequired(x => x.Receiver).WithMany(x => x.ReceiveMessages).HasForeignKey(x => x.ReceiverId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ReceiverOfNotification>().HasKey(x => new { x.ReceiverId, x.ReceiveNotificationId });
            modelBuilder.Entity<ReceiverOfNotification>().HasRequired(x => x.Receiver).WithMany().HasForeignKey(x => x.ReceiverId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ReceiverOfNotification>().HasRequired(x => x.ReceiveNotification).WithMany().HasForeignKey(x => x.ReceiveNotificationId);
        }
    }


}
