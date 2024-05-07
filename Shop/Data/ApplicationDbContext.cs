using Microsoft.EntityFrameworkCore;
using Shop.Models.Entities;

namespace Shop.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        }

        public DbSet<Product> Products{ get; set; }
    }
}
