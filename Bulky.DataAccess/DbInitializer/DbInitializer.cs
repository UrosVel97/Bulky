using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly UserManager<IdentityUser> _userManager; //Ovaj objekat ce biti zaduzen za kreiranje korisnika u bazi

        private readonly RoleManager<IdentityRole> _roleManager; //Ovaj objekat ce biti zaduzen za kreiranje uloga u bazi

        private readonly ApplicationDbContext _db; //Ovaj objekat ce biti zaduzen za dodavanje migracija i push-ovanje na bazu

        public DbInitializer(UserManager<IdentityUser> user, RoleManager<IdentityRole> role,
            ApplicationDbContext db)
        {
            _userManager = user;
            _roleManager = role;
            _db = db;
        }


        //Ova metoda ce da bude zaduzena da Seed-uje nasu bazu podataka
        public void Initialize()
        {


            /*Prvo proveravamo da li postoje neke nove migracije u nasoj aplikaciji
             * i da nisu push-ovane u bazu. Ako postoje onda ih push-ujemo u bazu.*/
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception e)
            { }

            /*Koristimo objekat '_roleManager' klase 'RoleManager<>' kako bismo proverili da li uloga 'Role_Customer' postoji u bazi podataka. 
              Ako uloga ne postoji, onda najverovbatnije nijedna druga uloga ne postoji, pa cemo kreirati sve uloge koje su nam potrebne.
              Posto je metoda 'CreateAsync()' Asinhrona metoda onda mi moramo da sacekamo odgovor od te metode. 
              Mi mozemo da koristimo kljucnu rec 'await' koje je ispred poziva metode, ili mozemo da 
              koristimo metodu '.GetAwaiter().GetResult()' koja isto radi kao i kljucna rec 'await'.*/
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();



                /*Kreiramo korisnika sa ulogom 'Admin'. Metoda 'CreateAsync()' prima dva paramatra
                  prvi je objekat klase koja nasledjuje klasu 'IdentityUser' a drugi je sifra korisnika
                kada hoce da se login-uje. Posto je metoda asinhrona, onda umesto kljucne reci 'await' 
                mi cemo koristiti metode 'GetAwaiter().GetResult()'.*/
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Uros Velickovic",
                    PhoneNumber = "1112223333",
                    StreetAddress = "test 123 Ave",
                    State = "Srbija",
                    PostalCode = "18400",
                    City = "Chicago"
                }, "Test@1234").GetAwaiter().GetResult();

                //Kada se Admin korisnik kreira u bazi, onda cemo ga vratiti iz baze i dodeliti mu ulogu
                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();


            }

            return;
        }


    }
}
