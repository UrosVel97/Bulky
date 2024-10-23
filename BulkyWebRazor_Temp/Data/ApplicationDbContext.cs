using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        //Ova metoda nam sluzi da seed-ujemo (dodeljujemo slogove tabeli) u bazi podataka.
        //U ovom primeru mi u tabeli 'Category' hocemo da dodamo 3 sloga gde ce prvi slog da
        //ima vrednosti kolone: id=1,name='Action',displayorder=1.
        //drugi slog imace vrednosti kolona: id=2,name='SciFi',displayorder=1.
        //treci slog imace vrednosti kolona: id=3,name='History',displayorder=1.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category(1, "Action", 1),
                new Category(2, "SciFi", 1),
                new Category(3, "History", 1));

        }
    }
}
