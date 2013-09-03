using System.Data.Entity;

namespace Notifications.DataAccessLayer
{
    public class ContextNotifications : DbContext
    {
        public ContextNotifications()
            : base("Data Source=(localdb)\\v11.0;Initial Catalog=Notifications.DataAccessLayer.ContextNotifications;Integrated Security=True;MultipleActiveResultSets=True;")
        {
        }

        public DbSet<SqlEmployee> Employees { get; set; }
        public DbSet<SqlNotification> Notifications { get; set; }
        public DbSet<SqlMessage> Messages { get; set; }
        public DbSet<SqlReceiversOfNotification> ReceiversOfNotifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlEmployee>().HasKey(x => x.EmployeeId);
            modelBuilder.Entity<SqlNotification>().HasKey(x => x.NotificationId);
            modelBuilder.Entity<SqlMessage>().HasKey(x => x.MessageId);
            modelBuilder.Entity<SqlReceiversOfNotification>().HasKey(x => x.ReceiversOfNotificationId);

            modelBuilder.Entity<SqlNotification>()
                .HasRequired(x => x.Sender)
                .WithMany(x => x.SendNotifications)
                .HasForeignKey(x => x.SenderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SqlMessage>()
                .HasRequired(x => x.Sender)
                .WithMany(x => x.SendMessages)
                .HasForeignKey(x => x.SenderId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<SqlMessage>()
                .HasRequired(x => x.Receiver)
                .WithMany(x => x.ReceiveMessages)
                .HasForeignKey(x => x.ReceiverId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SqlReceiversOfNotification>()
                .HasRequired(x => x.Receiver)
                .WithMany(x => x.ReceiveNotifications)
                .HasForeignKey(x => x.ReceiverId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<SqlReceiversOfNotification>()
                .HasRequired(x => x.ReceivingNotification)
                .WithMany(x => x.Receivers)
                .HasForeignKey(x => x.NotificationId)
                .WillCascadeOnDelete(false);
        }
    }
}