using Clima.Models;
using Microsoft.EntityFrameworkCore;

namespace Clima.Data
{
    public class AppDbContext(IConfiguration _config) : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
        }
        public DbSet<Tb_registos> Tb_Registos { get; set; }
    }
}
