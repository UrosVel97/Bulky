using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory.IReporsitory
{
    public interface IOrderHeaderReporsitory : IReporsitory<OrderHeader>
    {
        void Update(OrderHeader obj);// Metoda za azuiranje sloga u tabeli

        //Metoda za azuriranje statusa porudzbine i statusa placanja.
        void UpdateStatus(int id, string orderStatus, string? paymentStatus=null);


		//Metoda za dodeljivanje vrednosti propertijima 'SessionId' i 'PaymentIntentId' klase 'OrderHeader'
		void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
    }
}
