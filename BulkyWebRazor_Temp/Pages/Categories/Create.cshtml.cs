using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    /*Da ne bi pisali za svaki objekat klase 'CreateModel' da ce mu se dodeliti vrednosti iz forme, mi mozemo
     * iznad klase da napisemo anotaciju podataka koja ce naglasiti da svi objekti u klasi mogu sluziti za dodelu
     * podataka iz forme*/
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        /*Da bi koristili neki model u .cshtml stranici prvo moramo da 
         * ga deklarisemo u .cs fajlu  zatim inicijalizujemo u 'OnGet' metodi.
         * Na taj nacin mi cemo moci da pristupimo tim podacima*/
    [BindProperty]//Ova anotacija podataka naglasava da ce vrednosti polja iz forme biti dodeljena propertijima objekta 'Kategorija'
        public Category Kategorija { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            
            
        }
        /* 'Create.cshtml' stranica ima formu koja je tipa 'POST' i ta forma ima input 
         * polja koja mogu da se popune. Mi moramo da osiguramo da ce ta polja biti dodeljena
         * objektu koji je tipa 'Category' i da ce metoda 'OnPost' moci da azuirira bazu na osnovu
         * tog objekta. Posto nam je forma tipa 'POST' onda moramo da imamo i 'OnPost' 
         * metodu u 'CreateModel' klasi koja ce da azurira bazu. Deklarisali smo 'Kategorija' 
         * objekat koji je tipa 'Category', neophodno je da stavimo anotaciju podataka '[BindProperty]'
         * iznad deklaracije objekta, tako da kada se polja u 'Create.cshtml' fajlu popune ona ce 
         * se dodeliti propertijima objekta 'Kategorija'. Metoda 'OnPost' ne mora da ima atribute
         * vec ce da koristi javni objekat 'Kategorija' cijim je propertijima dodeljena vrednost 
         * kada je kliknuto 'Submit' dugme u formi.*/
        public IActionResult OnPost()
        {
            _db.Categories.Add(Kategorija);
            _db.SaveChanges();
            TempData["success"] = "Kategorija je uspesno kreirana!!";/* Ovaj 'Dictionary' 'TempData' nam sluzi da definisemo kljuc i vrednost
                                                                          * koji je asociran sa tim kljucem. Ta vrednost ce se prikazati samo jednom 
                                                                          * na stranici na koju smo se preusmerili. Kada ponovo azuriramo tu stranicu na koju smo se
                                                                          * preusmerli ta vrednost se vise nece pokazati.*/
            return RedirectToPage("Index");
        }
    }
}
