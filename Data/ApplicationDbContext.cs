using Microsoft.EntityFrameworkCore;
using SubdivisionManagement.Models;

namespace SubdivisionManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure SQLite-compatible data types
            // For User entity
            modelBuilder.Entity<User>(entity => 
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.LastName).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.Email).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.PhoneNumber).HasColumnType("TEXT");
                entity.Property(e => e.Address).HasColumnType("TEXT");
                entity.Property(e => e.BlockLot).HasColumnType("TEXT");
                entity.Property(e => e.IsActive).HasColumnType("INTEGER");
                entity.Property(e => e.CreatedAt).HasColumnType("TEXT");
                entity.Property(e => e.LastLogin).HasColumnType("TEXT");
                
                // Add unique constraint for email
                entity.HasIndex(u => u.Email).IsUnique();
            });
            
            // For Announcement entity
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.Content).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnType("TEXT");
                entity.Property(e => e.IsActive).HasColumnType("INTEGER");
            });
            
            // For Request entity
            modelBuilder.Entity<Request>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Description).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.Status).HasColumnType("INTEGER");
                entity.Property(e => e.CreatedAt).HasColumnType("TEXT");
                entity.Property(e => e.UpdatedAt).HasColumnType("TEXT");
                entity.Property(e => e.AdminNotes).HasColumnType("TEXT");
            });

            // Seed default admin user
            var adminPasswordHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes("Admin123!")));

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@verdenest.com",
                    PasswordHash = adminPasswordHash,
                    Role = UserRoleType.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Configure relationships
            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 