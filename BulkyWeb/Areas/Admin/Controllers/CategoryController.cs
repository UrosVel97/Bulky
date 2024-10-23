using Bulky.DataAccess;
using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    //Moramo da naglasimo u koj 'Area' se nalazi kontroler
    [Area("Admin")]
    public class CategoryController : Controller
    {

        /* Umesto 'ICategoryReporsitory' interfejsa, koristicemo promenljivu interfejsa
         * 'IUnitOfWork' za manipulaciju baze.*/
        private readonly IUnitOfWork _unit;

        public CategoryController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        #region VracanjeSvihKategorijaIzBaze
        //Metoda za vracanje svih kategorija iz baze
        public IActionResult Index()
        {
            List<Category> listaKategorija = _unit.Category.GetAll().ToList();
            return View(listaKategorija);
        }
        #endregion

        #region MetodeZaKreiranjeKategorije
        public IActionResult Create()
        {
            return View();
        }

        /*Metoda 'Create' mora da ima isti naziv kao odgovarajuci '.cshtml' fajl. U suprotnom
          kada pogled prosledjuje podatke 'Action' metodi ako ta metoda nema isti naziv kao fajl onda
          podaci nece ni stici do te metode.*/
        /*Ako u .cshtml fajlu imamo formu koja je tipa 'POST' onda i 'Action' metoda u kontroleru
          mora da se naglasi da je 'POST', tako sto cemo iznad metode u uglastim zagradama da naglasimo
          tip metode.*/
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                //U pogledu 'Create.cshtml' kod polja za unos naziva kategorije bice
                //prikazana specijalna poruka o gresci ako polje 'Naziv Kategorije' i polje
                //'Redosled Prikazivanja' imaju istu vrednost.
                ModelState.AddModelError("name", "Polje 'Redosled Prikazivanja' ne sme da ima istu vrednost kao polje 'Naziv kategorije'!!");
            }

            if (ModelState.IsValid) //Ova promenljiva tipa 'bool' sadrzi informaciju da li propertiji objekta koji je prosledjen su postovali ogranicenja i da li su validni.
            {
                _unit.Category.Add(obj);/*Naglasili smo da hocemo da dodamo novi objekat u tabelu u bazi. Da 
                                     bi objekat bio dodat mi moramo da pozovemo drugu metodu.*/
                _unit.Save();//Sve promene koje smo uradili u projektu ce se proslediti u bazi pomocu ove metode.

                TempData["success"] = "Kategorija je uspesno krerana!!"; /* Ovaj 'Dictionary' 'TempData' nam sluzi da definisemo kljuc i vrednost
                                                                          * koji je asociran sa tim kljucem. Ta vrednost ce se prikazati samo jednom 
                                                                          * na stranici na koju smo se preusmerili. Kada ponovo azuriramo tu stranicu na koju smo se
                                                                          * preusmerli ta vrednost se vise nece pokazati.*/



                return RedirectToAction("Index", "Category");/*Ova metoda sluzi da nas preusmerii na 
                                                         neki odredjeni pogled u nekom odredjenom kontroleru.*/
            }



            return View();


        }
        #endregion


        #region MetodeZaEditovanjeKategorije
        //Metoda koja prikazuje pogled kategorije koju hocemo da editujemo
        [HttpGet]
        public IActionResult Edit(int? id) //Kada je ovako deklarisana promenljiva tipa 'int' onda promenljiva moze da ima 'null' vrednost
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //3 nacina za vracanje objekta iz baze pomocu id-a
            Category category1 = _unit.Category.Get(u => u.Id == id);// Koristimo Linq izraz za pronalazenje elementa iz baze
            return View(category1);
        }


        //Metoda koja prima objekat ciji su podaci izmenjeni i azurira bazu na osnovu tog objekta
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unit.Category.Update(obj);
                _unit.Save();
                TempData["success"] = "Kategorija je uspesno azuirirana!!";/* Ovaj 'Dictionary' 'TempData' nam sluzi da definisemo kljuc i vrednost
                                                                          * koji je asociran sa tim kljucem. Ta vrednost ce se prikazati samo jednom 
                                                                          * na stranici na koju smo se preusmerili. Kada ponovo azuriramo tu stranicu na koju smo se
                                                                          * preusmerli ta vrednost se vise nece pokazati.*/

                return RedirectToAction("Index");
            }

            return View();
        }
        #endregion

        #region MetodeZaBrisanjeKategorije
        //Metoda koja prikazuje pogled kategorije koju hocemo da obrisemo
        [HttpGet]
        public IActionResult Delete(int? id) //Kada je ovako deklarisana promenljiva tipa 'int' onda promenljiva moze da ima 'null' vrednost
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }


            Category category1 = _unit.Category.Get(u => u.Id == id);

            return View(category1);
        }



        [HttpPost, ActionName("Delete")] //Kada imamo da dve ili vise metoda imaju istu deklaraciju(potpis) onda 
                                         // je dobro da naglasimo na koju metodu tacno mislimo kada hocemo iz nekog pogleda da je pozovemo.
                                         // U ovoj situaciji posto je u pogledu 'Delete.cshtml' forma tipa 'POSTT' onda kada kliknemo
                                         //na dugme 'submit' aplikacija ce znati da treba da nadje metodu 'Delete' i ima u uglastim zagradama 
                                         // iznad nje naglaseno da je tipa 'HttpPost'.
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unit.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unit.Category.Remove(obj);
            _unit.Save();
            TempData["success"] = "Kategorija je uspesno obrisana!!";/* Ovaj 'Dictionary' 'TempData' nam sluzi da definisemo kljuc i vrednost
                                                                          * koji je asociran sa tim kljucem. Ta vrednost ce se prikazati samo jednom 
                                                                          * na stranici na koju smo se preusmerili. Kada ponovo azuriramo tu stranicu na koju smo se
                                                                          * preusmerli ta vrednost se vise nece pokazati.*/


            return RedirectToAction("Index");




        }

        #endregion



    }
}
