using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Model;

namespace EmployeeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // 🔥 This connects to Employees table
        public DbSet<Employee> Employees { get; set; }
    }
}
