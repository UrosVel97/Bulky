using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        /*Da bi koristili neki model u .cshtml stranici prvo moramo da 
         * ga deklarisemo u .cs fajlu  zatim inicijalizujemo u 'OnGet' metodi.
         * Na taj nacin mi cemo moci da pristupimo tim podacima*/
        public List<Category> Categories { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
                _db = db;
        }
        public void OnGet()
        {
            //Vracanje svih kategorija iz baze
            Categories = _db.Categories.ToList();
        }
    }
}
