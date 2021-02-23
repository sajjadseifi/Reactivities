using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Photo> Photos { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<Value>().HasData(
                new Value { Id = 1, Name = "Value 10.1" },
                new Value { Id = 2, Name = "Value 10.2" },
                new Value { Id = 3, Name = "Value 10.3" },
                new Value { Id = 4, Name = "Value 10.4" }
            );

            builder.Entity<UserActivity>(
                x => x.HasKey(
                    ua => new
                    {
                        ua.AppUserId,
                        ua.ActivityId
                    }
                )
            );

            builder.Entity<UserActivity>()
            .HasOne(u => u.AppUser)
            .WithMany(a => a.UserActivity)
            .HasForeignKey(u => u.AppUserId);

            builder.Entity<UserActivity>()
            .HasOne(a => a.Activity)
            .WithMany(u => u.UserActivity)
            .HasForeignKey(a => a.ActivityId);
        }
    }
}