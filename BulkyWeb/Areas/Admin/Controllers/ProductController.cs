﻿using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    //Obavezno mora da se naglasi kojoj 'Area-i' pripada kontroler
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]//Sa ovom anotacijom podataka smo naglasili da samo korisnik koji ima ulogu "Admin" moze da pristupi Action metodama ovog kontrolera
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unit;
        /*Da bi mogli da pristupimo folderima i fajlovima u 'Solution Explorer-u', moramo
         * da deklarišemo objekat klase 'IWebHostEnvironment' koja ce nam sluziti bas za pristup
         * folderima i fajlovima u nasim projektima.
         * */
        private readonly IWebHostEnvironment _webHostEnvironment;

        /*Servis za objekat klase 'IWebHostEnvironment' je vec registrovan u Dependecy Injection Container-u,
         * tako da necemo imati problem ako stavimo kao parametar objekat klase 'IWebHostEnvironment' u konstruktoru.
         * */

        public ProductController(IUnitOfWork unit, IWebHostEnvironment webHostEnvironment)
        {
            this._unit = unit;
            this._webHostEnvironment = webHostEnvironment;
        }

        #region VratiSveProizvode
        [Authorize(Roles = SD.Role_Admin)]//U slucaju ako ne zelimo da korisnik ne moze da pristupi svim action metodama vec samo nekim, onda mozemo iznad Action metode da stavimo anotaciju podataka '[Authorize()]'
        public IActionResult Index()
        {
            //Posto objekat tipa 'Product' ima navigation properti tipa 'Category', onda moramo i da naglasimo
            //da cemo i njega da vracamo i da mu dodelimo vrednosti. Da objekat tipa 'Product' ima vise
            //navigation propertija, onda bi ih sve odvojili zarezom. Primer 'GetAll(includeProperties:"Category,Model2,Model3")'
            List<Product> products = this._unit.Proizvod.GetAll(includeProperties: "Category").ToList();
            return View(products);
        }
        #endregion

        #region Kreiraj_Ili_Azuriraj_Proizvod

        // U ovoj Action Metodi smo kombinovali Create and Update. Tako da jedna metoda i
        // jedan pogled ce se koristiti i za kreiranje i za azuiranje proizvoda.
        // 'Upsert' je kombinacija reci 'Update' i 'Insert'
        public IActionResult Upsert(int? id)
        {

            /*Kreiramo novi objekat tipa 'ProductVM' i dodeljujemo vrednosti njegovim
            propertijima 'ListaKategorija' i 'Product'.
            Objekat klase 'SelectListItem' ima dva propertija 'Text' i 'Value'.
            Propertju 'Text' klase 'SelectListItem' dodelicemo vrednost propertija
            'Name' klase 'Category'. Propertiju 'Value' klase 'SelectListItem' dodelicemo
            vrednost propertija 'Id' klase 'Category'.*/
            ProductVM productVM = new()
            {
                ListaKategorija = _unit.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                //kreiraj proizvod
                return View(productVM);
            }
            else
            {
                //azuriraj proizvod
                productVM.Product = _unit.Proizvod.Get(u => u.Id == id);
                return View(productVM);
            }


        }





        [HttpPost]//Iz pogleda se vraca objekat klase 'ProductVM' i vraca se fajl koji je
                  //krajnji korisnik selektovao sa svog Desktop racunara.
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            /* Validacija na strani servera se vrši na sve propertije
             * klase 'ProductVM'. Mi moramo da naglasimo za koje propertije mi
             * ne zelimo da vrsimo validaciju. To se radi tako sto iznad propertija
             * klase stavimo anotaciju podataka '[ValidateNever]'.*/
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath; //Vracamo apsolutnu putanju do 'wwwroot' direktorijuma 

                if (file != null)
                {
                    /* Svaki put kada krajnji korisnik ucita neki fajl sa Desktop racunara, 
                     * mi cemo kreirati novi fajl koji ce imati neki random 
                     * naziv i koji ce cuvati podatke tog fajla koji smo ucitali sa Desktopa 
                     */
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //Kreiramo random naziv za fajl i nadovezujemo uz naziv i ekstenziju fajla
                    string productPath = Path.Combine(wwwRootPath, @"Slike\products");//Promenljiva 'productPath' cuva apsolutnu putanju do direktorijuma gde cemo cuvati slike proizvoda

                    if (!string.IsNullOrEmpty(productVM.Product.ImageURL)) //Ako properti 'ImageURL' nije null onda ulazimo u telo if klauzule
                    {
                        //Brisemo staru sliku. Promenljiva 'oldImagePath' ce cuvati putanju do stare slike, samo sto naziv putanje mora biti bez kose crte u levo '\\'. Prva kosa crta je escape karakter za drugu.
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageURL.TrimStart('\\'));

                        //Ako postoji fajl u Solution Exploreru sa zadatim nazivom, onda udji u telo 'if' klauzule
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            //U slucaju ako postoji fajl u Solution Exploreru sa odgovarajucim nazivom, onda ga brisemo
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)) //Kreiramo novi fajl na putanji 'productPath\fileName', i u njega prekopiramo bitove(binarnu vrednost) fajla koji smo ucitali sa Desktop racunara
                    {
                        file.CopyTo(fileStream);//Kopiramo binarnu vrednost fajla koji smo ucitali sa Desktop racunara u novi fajl koji smo kreirali sa random imenom

                    }

                    productVM.Product.ImageURL = @"\Slike\products\" + fileName; //U propertiju 'ImageURL' klase 'Product', cuvamo samo tacan naziv fajla
                }

                if (productVM.Product.Id == 0)
                {
                    _unit.Proizvod.Add(productVM.Product);
                }
                else
                {
                    _unit.Proizvod.Update(productVM.Product);
                }

                _unit.Save();// Cuva podatke u bazu

                return RedirectToAction("Index", "Product");
            }
            else
            {
                /*U slucaju ako podaci nisu validni, mi moramo ponovo da instanciramo listu 'ListaKategorija'
                 * i ceo objekat 'productVM' da prosledimo pogledu ponovo. */
                productVM.ListaKategorija = _unit.Category.GetAll().Select(u =>
            new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

                return View(productVM);
            }


        }

        #endregion

        #region IzbrisiProizvod
        public IActionResult IzbrisiProizvod(int? id)
        {
            Product proizvod = _unit.Proizvod.Get(u => u.Id == id);


            return View(proizvod);
        }


        [HttpPost]
        public IActionResult IzbrisiProizvod(Product proizvod)
        {
            _unit.Proizvod.Remove(proizvod);
            _unit.Save();

            return RedirectToAction("Index", "Product");
        }

        #endregion



        #region API CALLS


        //API poziv za brisanje sloga iz tabele tipa 'Product'
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unit.Proizvod.Get(u => u.Id == id); //Vracamo proizvod koji se brise
            if (productToBeDeleted == null)
            {
                //U slucaju ako ne postoji proizvod sa zadatim id-jem u bazi, prikazi poruku o gresci
                return Json(new { success = false, message = "Greska u toku brisanja proizvoda!" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageURL.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                //U slucaju ako fajl, tj. slika, postoji sa zadatim nazivom, mi tu sliku brisemo
                System.IO.File.Delete(oldImagePath);
            }

            _unit.Proizvod.Remove(productToBeDeleted);
            _unit.Save();

            return Json(new { success = true, message = "Delete succesful" });



        }
        #endregion



    }
}

