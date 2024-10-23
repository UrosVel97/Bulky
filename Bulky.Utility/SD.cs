using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    //Staticki podaci web aplikacije
    public static class SD
    {
		//Uloge koje sve korisnik, kada se registruje, moze da dobije
        public const string Role_Customer= "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Company = "Company";


		//Order Status - status porudzbine
        public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusShipped = "Shipped";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";


		//Payment Status - Status vezano za placanje
		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusDelayedPayement = "ApprovedForDelayedPayement";
		public const string PaymentStatusRejected = "Rejected";


		//Ova konstanta ce biti kljuc u hash tablici za Sesiju
		public const string SessionCart = "SessionShoppingCar";

	}
}
