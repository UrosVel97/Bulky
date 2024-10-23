using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{
    public class ProductReporsitory : Reporsitory<Product>, IProductReporsitory
    {

        private readonly ApplicationDbContext _db;

        public ProductReporsitory(ApplicationDbContext db) :base(db) 
        {
                this._db = db;
        }

        public void Update(Product product)
        {
            Product proizvod = _db.Products.FirstOrDefault(u => u.Id == product.Id);
            proizvod.ISBN= product.ISBN;
            proizvod.Title= product.Title;
            proizvod.Author= product.Author;
            proizvod.Description= product.Description;
            proizvod.Price50= product.Price50;
            proizvod.Price100= product.Price100;
            proizvod.ListPrice= product.ListPrice;
            proizvod.CategoryId= product.CategoryId;
            proizvod.Id= product.Id;

            //U slucaju ako 'ImageURL' nije null, onda ga azuriraj
            if (product.ImageURL != null)
            {
                proizvod.ImageURL = product.ImageURL;
            }

            _db.Products.Update(proizvod);
        }
    }
}
