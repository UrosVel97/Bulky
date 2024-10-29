using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{
    // Klasa koja implementira 'IUnitOfWork'
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _db;


        //Ovaj interfejs ce se koristiti za manipulaciju tabele 'Category'
        public ICategoryReporsitory Category { get; private set; }

        public IProductReporsitory Proizvod { get; private set; }

        public ICompanyReporsitory Kompanija { get; private set; }
        public IShoppingCartReporsitory ShoppingCart { get; private set; }

        public IApplicationUserReporsitory ApplicationUser { get; private set; }

        public IOrderDetailReporsitory OrderDetail { get; private set; }

        public IOrderHeaderReporsitory OrderHeader { get; private set; }

        public IProductImageReporsitory ProductImage { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this._db= db;
            Category = new CategoryReporsitory(db);
            Proizvod = new ProductReporsitory(db);
            Kompanija=new CompanyReporsitory(db);
            ShoppingCart=new ShoppingCartReporsitory(db);
            ApplicationUser=new ApplicationUserReporsitory(db);
            OrderHeader=new OrderHeaderReporsitory(db);
            OrderDetail=new OrderDetailReporsitory(db);
            ProductImage=new ProductImageReporsitory(db);

        }


        
        //Sve promene koje smo izvrsili guramo na bazu.
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
