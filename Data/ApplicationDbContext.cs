
using SnackMVCApp.Models;
using Microsoft.EntityFrameworkCore;

namespace SnackMVCApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Snack> Snacks { get; set; }

    }
}
