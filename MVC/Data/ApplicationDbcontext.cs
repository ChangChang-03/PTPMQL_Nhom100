using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

       
        public DbSet<Person> Persons { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<DaiLy> DaiLys { get; set; }

        public DbSet<HeThongPhanPhoi> HeThongPhanPhois { get; set; }

        public DbSet<Student> Students { get; set; }


    }
}
