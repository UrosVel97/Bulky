using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    //Obavezno mora da se naglasi kojoj 'Area-i' pripada kontroler
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Company + "," + SD.Role_Admin)]//Sa ovom anotacijom podataka smo naglasili da samo korisnik koji ima ulogu "Admin" moze da pristupi Action metodama ovog kontrolera
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(IUnitOfWork unit, IWebHostEnvironment webHostEnvironment)
        {
            this._unit = unit;
            this._webHostEnvironment = webHostEnvironment;
        }

        #region VratiSveKompanije
        public IActionResult Index()
        {
            List<Company> products = this._unit.Kompanija.GetAll().ToList();

            return View(products);
        }
        #endregion

        #region Kreiraj_Ili_Azuriraj_Kompaniju

        // U ovoj Action Metodi smo kombinovali Create and Update. Tako da jedna metoda i
        // jedan pogled ce se koristiti i za kreiranje i za azuiranje proizvoda.
        // 'Upsert' je kombinacija reci 'Update' i 'Insert'
        public IActionResult Upsert(int? id)
        {

            Company company = new Company();

            if (id == null || id == 0)
            {
                //kreiraj proizvod
                return View(company);
            }
            else
            {
                //azuriraj proizvod
                company = _unit.Kompanija.Get(u => u.Id == id);
                return View(company);
            }


        }





        [HttpPost]//Iz pogleda se vraca objekat klase 'ProductVM' i vraca se fajl koji je
                  //krajnji korisnik selektovao sa svog Desktop racunara.
        public IActionResult Upsert(Company company)
        {
            /* Validacija na strani servera se vrši na sve propertije
             * klase 'ProductVM'. Mi moramo da naglasimo za koje propertije mi
             * ne zelimo da vrsimo validaciju. To se radi tako sto iznad propertija
             * klase stavimo anotaciju podataka '[ValidateNever]'.*/
            if (ModelState.IsValid)
            {


                if (company.Id == 0)
                {
                    _unit.Kompanija.Add(company);
                }
                else
                {
                    _unit.Kompanija.Update(company);
                }

                _unit.Save();// Cuva podatke u bazu

                return RedirectToAction("Index", "Company");
            }

            return View(company);
        }

        #endregion

        #region API CALLS


        //API poziv za brisanje sloga iz tabele tipa 'Company'
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unit.Kompanija.Get(u => u.Id == id); //Vracamo proizvod koji se brise
            if (companyToBeDeleted == null)
            {
                //U slucaju ako ne postoji proizvod sa zadatim id-jem u bazi, prikazi poruku o gresci
                return Json(new { success = false, message = "Greska u toku brisanja proizvoda!" });
            }





            _unit.Kompanija.Remove(companyToBeDeleted);
            _unit.Save();

            return Json(new { success = true, message = "Delete succesful" });



        }
        #endregion
    }


}