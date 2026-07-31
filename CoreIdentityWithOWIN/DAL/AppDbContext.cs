using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CoreIdentityWithOWIN.Models;

namespace CoreIdentityWithOWIN.DTOS
{
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> op) : base(op)
        {
        }

        public virtual DbSet<MemberType> MemberTypes { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId });
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

           
            builder.Entity<Member>(entity =>
            {
                entity.Property(r => r.RegFee).HasColumnType("decimal(18,4)");

               
                entity.HasOne(m => m.MemberType)
                    .WithMany(mt => mt.Members)
                    .HasForeignKey(m => m.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

       
            builder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.Member)
                    .WithMany(m => m.Transactions)
                    .HasForeignKey(t => t.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Seed();
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.Entity<MemberType>().HasData(
                new MemberType { TypeId = 1, Title = "New Member" },
                new MemberType { TypeId = 2, Title = "Regular Member" },
                new MemberType { TypeId = 3, Title = "Premium Member" }
            );
        }
    }
}