using Microsoft.EntityFrameworkCore;

namespace ScholaAi.Models
{
    public class DBcontext : DbContext
    {
        public DBcontext()
        {
        }
        public DBcontext(DbContextOptions<DBcontext> options) : base(options) { }
        public DbSet<adminLogs>adminLogs { get; set; }
        public DbSet<chatMessage>chatMessages { get; set; }
        public DbSet<notification>notifications { get; set; }
        public DbSet<rating>ratings { get; set; }
        public DbSet<requestBroadcast>requestBroadcasts { get; set; }
        public DbSet<session>sessions { get; set; }
        public DbSet<sessionRequest>sessionRequests { get; set; }
        public DbSet<student>students { get; set; }
        public DbSet<subject>subjects { get; set; }
        public DbSet<teacher>teachers { get; set; }
        public DbSet<teacherSubject>teacherSubjects { get; set; }
        public DbSet<transaction>transactions { get; set; }
        public DbSet<user>users { get; set; }
        public DbSet<wallet>wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<user>()
                .HasIndex(x => x.email)
                .IsUnique();

            modelBuilder.Entity<user>()
                .HasOne(u => u.adminLogs)
                .WithOne(a => a.target)
                .HasForeignKey<adminLogs>(t => t.targetUserId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<sessionRequest>()
                .HasOne(u => u.adminLogs)
                .WithOne(a => a.targetRequest)
                .HasForeignKey<adminLogs>(t => t.targetRequestId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<student>()
                .HasKey(s => s.userId); 

            modelBuilder.Entity<student>()
                .HasOne(s => s.user)
                .WithOne(u => u.student)
                .HasForeignKey<student>(s => s.userId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<teacher>()
                .HasKey(t => t.userId);

            modelBuilder.Entity<teacher>()
                .HasOne(t => t.user)
                .WithOne(u => u.teacher)
                .HasForeignKey<teacher>(t => t.userId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<wallet>()
                .HasOne(w => w.user)
                .WithOne(u => u.wallet)
                .HasForeignKey<wallet>(w => w.userId)
                 .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<user>()
                .HasIndex(u => u.email)
                .IsUnique();

            modelBuilder.Entity<user>()
                .HasIndex(u => u.userName)
                .IsUnique();

            
            modelBuilder.Entity<chatMessage>()
                .HasOne(m => m.sender)
                .WithMany(u => u.sentMessages)
                .HasForeignKey(m => m.senderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<chatMessage>()
                .HasOne(m => m.receiver)
                .WithMany(u => u.receivedMessages)
                .HasForeignKey(m => m.receiverId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<notification>()
                .HasOne(n => n.sender)
                .WithMany(u => u.sentNotifications)
                .HasForeignKey(n => n.senderId)
                .OnDelete(DeleteBehavior.NoAction);

            
            modelBuilder.Entity<notification>()
                .HasOne(n => n.receiver)
                .WithMany(u => u.receivedNotifications)
                .HasForeignKey(n => n.receiverId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<wallet>()
               .HasOne(w => w.user)
               .WithOne(u => u.wallet)
               .HasForeignKey<wallet>(w => w.userId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<sessionRequest>()
                .HasOne(r => r.student)
                .WithMany(s => s.requests)
                .HasForeignKey(r => r.studentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<sessionRequest>()
                .HasOne(r => r.teacher)
                .WithMany(t => t.sessionRequests)
                .HasForeignKey(r => r.teacherId)
                .OnDelete(DeleteBehavior.SetNull);

           
            modelBuilder.Entity<sessionRequest>()
                .HasOne(r => r.session)
                .WithOne(s => s.sessionRequest)
                .HasForeignKey<session>(s => s.requestId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<session>()
                .HasOne(s => s.teacher)
                .WithMany(t => t.sessions)
                .HasForeignKey(s => s.teacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<session>()
                .HasOne(s => s.student)
                .WithMany(st => st.sessions)
                .HasForeignKey(s => s.studentId)
                .OnDelete(DeleteBehavior.NoAction);

            
            modelBuilder.Entity<session>()
                .HasOne(s => s.rating)
                .WithOne(r => r.session)
                .HasForeignKey<rating>(r => r.sessionId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<session>()
                .HasOne(s => s.transaction)
                .WithOne(t => t.session)
                .HasForeignKey<transaction>(t => t.sessionId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<notification>()
                .HasOne(n => n.sessionNotification)
                .WithMany(s => s.notifications)
                .HasForeignKey(n => n.sessionId)
                 .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<requestBroadcast>()
                .HasOne(rb => rb.teacher)
                .WithMany(t => t.requestBroadcasts)
                .HasForeignKey(rb => rb.teacherId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<requestBroadcast>()
                .HasOne(rb => rb.teacherSession)
                .WithMany(r => r.requestBroadcasts)
                .HasForeignKey(rb => rb.requestId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<rating>()
                .HasOne(r => r.teacher)
                .WithMany(t => t.ratings)
                .HasForeignKey(r => r.teacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<transaction>()
                .HasOne(t => t.fromWallet)
                .WithMany(w => w.transactionsFrom)
                .HasForeignKey(t => t.fromWalletId)
                .OnDelete(DeleteBehavior.NoAction);

          
            modelBuilder.Entity<transaction>()
                .HasOne(t => t.toWallet)
                .WithMany(w => w.transactionsTo)
                .HasForeignKey(t => t.toWalletId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<teacherSubject>()
                .HasKey(r => new { r.subjectId, r.teacherId });
        }
    }
}
