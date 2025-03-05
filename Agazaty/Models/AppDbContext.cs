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

        }
        public DbSet<ApplicationUser> Users { get; set; }  
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<CasualLeave> CasualLeaves { get; set; }
        public DbSet<PermitLeave> PermitLeaves { get; set; }
        public DbSet<PermitLeaveImage> PermitLeaveImages { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SickLeave> SickLeaves { get; set; }
        public DbSet<NormalLeave> NormalLeaves { get; set; }
    }
}
