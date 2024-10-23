using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{
    public class OrderDetailReporsitory : Reporsitory<OrderDetail>, IOrderDetailReporsitory
    {

        private readonly ApplicationDbContext _db;

        public OrderDetailReporsitory(ApplicationDbContext db) :base(db) 
        {
            this._db=db;
        }
        
            
        
        public void Update(OrderDetail obj)
        {
            
            _db.OrderDetails.Update(obj);
        }
    }
}
