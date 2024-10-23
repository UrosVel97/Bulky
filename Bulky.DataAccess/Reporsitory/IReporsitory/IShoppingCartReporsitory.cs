using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory.IReporsitory
{
    public interface IShoppingCartReporsitory : IReporsitory<ShoppingCart>
    {
        void Update(ShoppingCart obj);// Metoda za azuiranje sloga u tabeli

    }
}
