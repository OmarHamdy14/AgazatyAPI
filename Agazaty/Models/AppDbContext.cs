using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Agazaty.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> op) : base(op)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SickLeave>()
                    .HasIndex(l => new { l.UserID, l.StartDate, l.EndDate });

            modelBuilder.Entity<NormalLeave>()
                .HasIndex(l => new { l.UserID, l.StartDate, l.EndDate });

            modelBuilder.Entity<CasualLeave>()
                .HasIndex(l => new { l.UserId, l.StartDate, l.EndDate });
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<CasualLeave> CasualLeaves { get; set; }
        public DbSet<PermitLeave> PermitLeaves { get; set; }
        public DbSet<PermitLeaveImage> PermitLeaveImages { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SickLeave> SickLeaves { get; set; }
        public DbSet<NormalLeave> NormalLeaves { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
    }
}