using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    //Ovaj model ce biti namenjen za 'KreirajProizvod.cshtml' pogled
    public class ProductVM
    {
        public Product Product { get; set; }

        /* Moramo da naglasimo da se ovaj properti ne validira
         * na strani servera. U suprotnom ako to ne uradimo, aplikacija
         * ce naci gresku.*/
        [ValidateNever]
        public IEnumerable<SelectListItem> ListaKategorija { get; set; }
    }
}
