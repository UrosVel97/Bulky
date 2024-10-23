using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        [BindProperty]
        public Category Kategorija { get; set; }

        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db)
        {
                _db = db;
        }
        public void OnGet(int? id)
        {
            Kategorija = _db.Categories.Find(id);
        }

        public IActionResult OnPost()
        {
            _db.Categories.Remove(Kategorija);
            _db.SaveChanges();
            TempData["success"] = "Kategorija je uspesno izbrisana!!";/* Ovaj 'Dictionary' 'TempData' nam sluzi da definisemo kljuc i vrednost
                                                                          * koji je asociran sa tim kljucem. Ta vrednost ce se prikazati samo jednom 
                                                                          * na stranici na koju smo se preusmerili. Kada ponovo azuriramo tu stranicu na koju smo se
                                                                          * preusmerli ta vrednost se vise nece pokazati.*/
            return RedirectToPage("Index");
        }
    }
}
