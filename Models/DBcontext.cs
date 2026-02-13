using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ScholaAi.Models
{
    public class DBcontext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DBcontext(DbContextOptions<DBcontext> options) : base(options) { }

        public DbSet<AdminLogs> AdminLogs { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RequestBroadcast> RequestBroadcasts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionRequest> SessionRequests { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Availability> Availability { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= Student =================
            modelBuilder.Entity<Student>()
                .HasKey(s => s.ApplicationUserId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= Teacher =================
            modelBuilder.Entity<Teacher>()
                .HasKey(t => t.ApplicationUserId);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.ApplicationUser)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Subject)
                .WithMany(s => s.Teachers)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= Wallet =================
            modelBuilder.Entity<Wallet>()
                .HasKey(w => w.ApplicationUserId);

            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.ApplicationUser)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(w => w.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= Chat =================
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= Notification =================
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Sender)
                .WithMany(u => u.SentNotifications)
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Receiver)
                .WithMany(u => u.ReceivedNotifications)
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.SessionNotification)
                .WithMany(s => s.Notifications)
                .HasForeignKey(n => n.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= AdminLogs =================
            modelBuilder.Entity<AdminLogs>()
                .HasOne(a => a.Admin)
                .WithMany(u => u.AdminActions)
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AdminLogs>()
                .HasOne(a => a.TargetUser)
                .WithMany(u => u.AdminTargets)
                .HasForeignKey(a => a.TargetUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AdminLogs>()
                .HasOne(a => a.TargetRequest)
                .WithOne(r => r.AdminLogs)
                .HasForeignKey<AdminLogs>(a => a.TargetRequestId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= Session =================
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Sessions)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Sessions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Rating)
                .WithOne(r => r.Session)
                .HasForeignKey<Rating>(r => r.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Transaction)
                .WithOne(t => t.Session)
                .HasForeignKey<Transaction>(t => t.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= SessionRequest =================
            modelBuilder.Entity<SessionRequest>()
                .HasOne(r => r.Student)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SessionRequest>()
                .HasOne(r => r.Teacher)
                .WithMany(t => t.SessionRequests)
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SessionRequest>()
                .HasOne(r => r.Session)
                .WithOne(s => s.SessionRequest)
                .HasForeignKey<Session>(s => s.RequestId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SessionRequest>()
                .HasOne(sr => sr.Subject)
                .WithMany(s => s.sessionRequests)
                .HasForeignKey(sr => sr.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= RequestBroadcast =================
            modelBuilder.Entity<RequestBroadcast>()
                .HasOne(rb => rb.Teacher)
                .WithMany(t => t.RequestBroadcasts)
                .HasForeignKey(rb => rb.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RequestBroadcast>()
                .HasOne(rb => rb.TeacherSession)
                .WithMany(r => r.RequestBroadcasts)
                .HasForeignKey(rb => rb.RequestId)
                .OnDelete(DeleteBehavior.NoAction);

            // ================= Transaction =================
            modelBuilder.Entity<Transaction>()
              .HasOne(t => t.FromWallet)
              .WithMany(w => w.TransactionsFrom)
              .HasForeignKey(t => t.FromWalletId)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ToWallet)
                .WithMany(w => w.TransactionsTo)
                .HasForeignKey(t => t.ToWalletId)
                .OnDelete(DeleteBehavior.NoAction);


            // ================= Availability =================
            modelBuilder.Entity<Availability>()
                .HasOne(a => a.ApplicationUser)
                .WithMany()
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
