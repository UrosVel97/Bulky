using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{

    public class ShoppingCartReporsitory : Reporsitory<ShoppingCart>, IShoppingCartReporsitory
    {


        private ApplicationDbContext _db;


        public ShoppingCartReporsitory(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }




        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }
    }
}
