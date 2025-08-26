using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementSystem.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Daily> Daily { get; set; }

        // �v���C�}���[�L�[�ݒ�
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Daily>()
                .HasKey(d => new { d.EmployeeId, d.DisplayDate });
        }
    }
}
