using System.Data.Entity;

namespace Notifications.DataAccessLayer
{
    public class ContextNotifications : DbContext
    {
        public DbSet<SqlEmployee> Employees { get; set; }
        public DbSet<SqlNotification> Notifications { get; set; }
        public DbSet<SqlMessage> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlEmployee>().HasKey(x => x.EmployeeId);
            modelBuilder.Entity<SqlNotification>().HasKey(x => x.NotificationId);
            modelBuilder.Entity<SqlMessage>().HasKey(x => x.MessageId);

            modelBuilder.Entity<SqlNotification>().HasRequired(x => x.Sender).WithMany(x => x.SendNotifications).HasForeignKey(x => x.SenderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SqlMessage>().HasRequired(x => x.Sender).WithMany(x => x.SendMessages).HasForeignKey(x => x.SenderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SqlMessage>().HasRequired(x => x.Receiver).WithMany(x => x.ReceiveMessages).HasForeignKey(x => x.ReceiverId).WillCascadeOnDelete(false);

            modelBuilder.Entity<SqlNotification>().HasMany(c => c.Receivers).WithMany(d => d.ReceiveNotification).Map(t => t.MapLeftKey("NotificationId").MapRightKey("EmployeeId").ToTable("ReceiversOfNotification"));
        }

    }
}