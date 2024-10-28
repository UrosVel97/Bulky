using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    public class RoleManagementVM
    {
        //Korisnik kome cemo da promenimo ulogu
        public ApplicationUser User { get; set; }

        public string RoleId { get; set; }
        
        //Lista uloga iz koje ce korisnik moci da izabere jednu
        public IEnumerable<SelectListItem> Roles { get; set; }

        public int CompanyId { get; set; }

        //Lista kompanija iz koje ce korisnik moci da izabere jednu kompaniju kojoj pripada. To je samo ako korisnik ima ulogu 'Company'
        public IEnumerable<SelectListItem> Companies { get; set; }

    }
}
