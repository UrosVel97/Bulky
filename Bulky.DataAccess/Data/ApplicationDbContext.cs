using Bulky.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; //Ovaj namespace nam omogucava da koristimo IdentityDbContext
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess
{
    /* Pre je nasa 'ApplicationDbContext' klasa nasledjivala 'DbContext' klasu, da bi mogli da kreiramo
     * 'Scaffold Identity' moramo da stavimo da nasledjuje 'IdentityDbContext' klasu. Tek kada kreiramo
     * 'Scaffold Identity' mi cemo da promenimo tako da klasa 'ApplicationDbContext' nasledjuje templejtsku nadklasu
     * 'IdentityDbContext<IdentityUser>'.
     * */
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        /*Ovbavezno moramo da kreiramo odgovarajuci DbSet<> za klasu koja nam sluzi kad ekstenzija klasi 'IdentityUser'.
         * Kada kreiramo migraciju i azuriramo bazu, nece se kreirati nova tabela koja se zove 'ApplicationUsers' vec
         * ce se u tabeli 'AspNetUsers' dodati nove kolone na koje su preslikani propertiji iz klase 'ApplicationUser'. */
        public DbSet<ApplicationUser>ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        //Ova metoda nam sluzi da seed-ujemo (dodeljujemo slogove tabeli) u bazi podataka.
        //U ovom primeru mi u tabeli 'Category' hocemo da dodamo 3 sloga gde ce prvi slog da
        //ima vrednosti kolone: id=1,name='Action',displayorder=1.
        //drugi slog imace vrednosti kolona: id=2,name='SciFi',displayorder=1.
        //treci slog imace vrednosti kolona: id=3,name='History',displayorder=1.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);//Ovu liniju koda moramo da napisemo kada radimo sa 'IdentityDbContext' klasom. To se ne mora pise kada radimo sa 'DbContext' klasom


            modelBuilder.Entity<Product>().HasData(
                new Product(10, "The way of the superior man", "A book about relationships", "a41fa2341",
                "David Deida", 30, 35, 25, 20, 1, ""),
                new Product(11, "The way of the superior man", "A book about relationships", "a41fa2341",
                "David Deida", 30, 35, 25, 20, 2, ""));

            modelBuilder.Entity<Category>().HasData(
               new Category(1, "Akcija", 1),
               new Category(2, "Triler", 2),
               new Category(3, "Misterija", 3));

        }
    }
}
