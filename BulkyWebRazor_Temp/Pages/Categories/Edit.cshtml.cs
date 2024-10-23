using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {

        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Category Kategorija { get; set; }


        /*Posto smo iz 'Index.cshtml' fajla prosledili 'id' modela koji zelimo da editujemo,
         * onda 'OnGet()' metoda moze da pribavi tu kategoriju iz baze i dodeli je objektu tipa 'Category'
         * koji se nalazi u klasi 'EditModel'.*/
        public void OnGet(int? id)
        {
            Kategorija = _db.Categories.Find(id);
            
            
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {

                _db.Categories.Update(Kategorija);
                _db.SaveChanges();

                TempData["success"] = "Kategorija je uspesno azuirirana!!";/* Ovaj 'Dictionary' 'TempData' nam sluzi da definisemo kljuc i vrednost
                                                                          * koji je asociran sa tim kljucem. Ta vrednost ce se prikazati samo jednom 
                                                                          * na stranici na koju smo se preusmerili. Kada ponovo azuriramo tu stranicu na koju smo se
                                                                          * preusmerli ta vrednost se vise nece pokazati.*/
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
