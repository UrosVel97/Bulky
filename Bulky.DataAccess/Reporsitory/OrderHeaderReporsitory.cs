using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{
    public class OrderHeaderReporsitory : Reporsitory<OrderHeader>, IOrderHeaderReporsitory
    {

        private readonly ApplicationDbContext _db;

        public OrderHeaderReporsitory(ApplicationDbContext db) :base(db) 
        {
            this._db=db;
        }
        
            
        
        public void Update(OrderHeader obj)
        {
            
            _db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {

            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            
            
            if (orderFromDb != null)
            {
                //Dodeljujemo novi status porudzbine. Moze imati vrednosti: 'Pending','Approved', 'Processing', 'Shipped'
                orderFromDb.OrderStatus = orderStatus;

				//Proveravamo ako parametar 'paymentStatus' nije null, onda azuiriramo u properti 'PaymentStatus' klase 'OrderHeader'
				if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            

            if (!string.IsNullOrEmpty(sessionId))
            {
				//Dodeljujemo vrednost propertiju 'SessionId' klase 'OrderHeader'
				orderFromDb.SessionId = sessionId;
            }
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				/*Dodeljujemo vrednost propertiju 'PaymentIntentId' klase 'OrderHeader' 
				 * i cuvamo tacan datum i vreme kada je placanje izvrseno */
				orderFromDb.PaymentIntentId = paymentIntentId;
				orderFromDb.PaymentDate = DateTime.Now;
			}
		}
	}
}
