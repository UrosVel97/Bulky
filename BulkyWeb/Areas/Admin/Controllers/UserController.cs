using Bulky.DataAccess;
using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BulkyWeb.Areas.Admin.Controllers
{
    //Obavezno mora da se naglasi kojoj 'Area-i' pripada kontroler
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]///Sa ovom anotacijom podataka smo naglasili da samo korisnik koji ima ulogu "Admin" moze da pristupi Action metodama ovog kontrolera
    public class UserController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly IUnitOfWork _unit;

        public UserController(IUnitOfWork unit, RoleManager<IdentityRole> role, UserManager<IdentityUser> user)
        {
            _unit = unit;
            _roleManager = role;
            _userManager = user;
        }




        public IActionResult Index()
        {

            List<ApplicationUser> users = _unit.ApplicationUser.GetAll(null,includeProperties: "Company").ToList();

            foreach (var user in users)
            {

                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            }


            return View(users);
        }



        public IActionResult RoleManagement(string id)
        {

            RoleManagementVM managementVM = new RoleManagementVM()
            {
                User = _unit.ApplicationUser.Get(u=>u.Id==id),

                Roles = _roleManager.Roles.Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id
                }),
                Companies = _unit.Kompanija.GetAll().Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })

            };




            return View(managementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM manage)
        {

            ApplicationUser user = _unit.ApplicationUser.Get(u => u.Id == manage.User.Id, includeProperties: "Company",tracked:true);

            if (user.Name != manage.User.Name)
            {

                user.Name = manage.User.Name;
                _unit.Save();
            }

            //Vracamo listu uloga koje su dodeljene korisniku
            IEnumerable<string> lista = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();

            //Vracamo naziv uloge koja je inicijalno dodeljena korisniku
            var role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            //Vracamo ulogu koju je korisnik selektovao u pogledu
            var selecteRole = _roleManager.Roles.Where(u => u.Id == manage.RoleId).FirstOrDefault();

            //Ako inicijalna uloga i selektovana uloga nisu iste, onda dodeljujemo novu ulogu korisniku
            if (role != selecteRole.Name)
            {
                //Uklanjamo staru ulogu koja je korisniku bila dodeljena
                _userManager.RemoveFromRoleAsync(user, role).GetAwaiter().GetResult();

                //dodeljujemo novu ulogu korisniku
                _userManager.AddToRoleAsync(user, selecteRole.Name).GetAwaiter().GetResult();

                //Ako je inicijalna uloga bila 'Company' onda brisemo strani kljuc 'CompanyId'
                if (selecteRole.Name != SD.Role_Company)
                {
                    user.CompanyId = null;
                }
                else
                {
                    user.CompanyId = manage.CompanyId;
                }

                _unit.ApplicationUser.Update(user);
                _unit.Save();
            }


            return RedirectToAction(nameof(Index));
        }



        //Metoda koja Zakljucava/Otkljucava korisnika, tako da on moze ili ne moze da se uloguje na nasu web-aplikaciju
        public IActionResult LockUnlock(string? id)
        {

            var objFromDb = _unit.ApplicationUser.Get(u => u.Id == id,null,tracked:true);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Greska pri Zakljucavanju/Otkljucavanju" });

            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //Korisnik je zakljucan i ne moze da se uloguje. Mi cemo da ga otkljucamo
                objFromDb.LockoutEnd = DateTime.Now; //Otkljucujemo korisnika
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000); //Zakljucujemo korisnika
            }

            _unit.Save(); //Posto je ukljucen tracking od strane EntityFramework Core-a onda ne moramo da pozivamo metodu '_db.ApplicationUsers.Update()'
            return RedirectToAction(nameof(Index));



        }

    }


}