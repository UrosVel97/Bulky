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

    public class ProductImageReporsitory : Reporsitory<ProductImage>, IProductImageReporsitory
    {

        /*Neophodno je ponovo deklarisati i definisati objekat klase 'ApplicationDbContext'
         * jer mora da se kreira objekat klase 'ApplicationDbContext' i dodeli konstruktoru
         * bazne klase 'Reporsitory<>'.*/
        private ApplicationDbContext _db;


        /*Kreiranje objekta klase 'ApplicationDbContext' i dodeljivanje konstruktoru 
         * bazne klase 'Reporsitory<>'*/
        public ProductImageReporsitory(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }



        //Azuriranje objekta u bazi
        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
