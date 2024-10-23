using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    public class ShoppingCartVM
    {
        //Lista svih artikla koje je korisnik stavio u korpu
        public IEnumerable<ShoppingCart> ShoppingCartList {  get; set; } 

        //Ukupnu cenu svih artikla u korpi ce cuvati properti 'OrderTotal' klase 'OrderHeader'
        public OrderHeader OrderHeader { get; set; }
       

    }
}
