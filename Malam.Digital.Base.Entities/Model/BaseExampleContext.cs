using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Malam.Digital.Base.Entities.Model
{
    public partial class BaseExampleContext : DbContext
    {
        public BaseExampleContext()
        {
        }

        public BaseExampleContext(DbContextOptions<BaseExampleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserStatus> UserStatus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=109.226.37.218,1433;Database= BaseExample; Trusted_Connection=False;user id=sa;password='1qaZXsw2'", x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeviceId).HasMaxLength(100);

                entity.Property(e => e.LastActivity).HasColumnType("datetime");

                entity.Property(e => e.LastModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.RefreshToken).HasMaxLength(150);

                entity.Property(e => e.SmsPassword).HasMaxLength(6);

                entity.Property(e => e.SysRowStatus).HasDefaultValueSql("((1))");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.InverseCreatedByUser)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_User_User1");

                entity.HasOne(d => d.LastModifyUser)
                    .WithMany(p => p.InverseLastModifyUser)
                    .HasForeignKey(d => d.LastModifyUserId)
                    .HasConstraintName("FK_User_User");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");

                entity.HasOne(d => d.UserStatus)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserStatus");
            });

            modelBuilder.Entity<UserStatus>(entity =>
            {
                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
