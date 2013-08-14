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

        public List<Notification> SendNotifications { get; set; }

        public List<Message> SendMessages { get; set; }

        public List<Message> ReceiveMessages { get; set; }

        public virtual List<Notification> ReceiveNotification { get; set; }
    }


    public class Notification
    {
        public Notification()
        {
            //Date = DateTime.Now;
            //Sender = new Employee();
            //Receivers = new List<Employee>();
        }

        public int NotificationId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public Employee Sender { get; set; }
        public int SenderId { get; set; }

        public virtual List<Employee> Receivers { get; set; }
    }



    public class Message
    {
        public Message()
        {
            Date = DateTime.Now;
            Sender = new Employee();
            Receiver = new Employee();
        }

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasKey(x => x.EmployeeId);
            modelBuilder.Entity<Notification>().HasKey(x => x.NotificationId);
            modelBuilder.Entity<Message>().HasKey(x => x.MessageId);

            modelBuilder.Entity<Notification>().HasRequired(x => x.Sender).WithMany(x => x.SendNotifications).HasForeignKey(x => x.SenderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>().HasRequired(x => x.Sender).WithMany(x => x.SendMessages).HasForeignKey(x => x.SenderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Message>().HasRequired(x => x.Receiver).WithMany(x => x.ReceiveMessages).HasForeignKey(x => x.ReceiverId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Notification>().HasMany(c => c.Receivers).WithMany(d => d.ReceiveNotification).Map(t => t.MapLeftKey("NotificationId").MapRightKey("EmployeeId").ToTable("ReceiversOfNotification"));
        }

    }

}
