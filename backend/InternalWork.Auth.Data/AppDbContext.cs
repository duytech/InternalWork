using InternalWork.Auth.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternalWork.Service
{
    public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public virtual DbSet<User> UserInfos { get; set; }

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(option =>
            {
                option.HasOne(u => u.AppIdentityUser)
                    .WithOne(i => i.User)
                    .HasForeignKey<User>(u => u.IdentityId);
            });
        }
    }
}
