using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
	//Properti ove klase cuvaju kljuceve za Stripe
	public class StripeSettings
	{
		public string SecretKey { get; set; }//Ovaj properti mora da ima isti naziv kao properti JSON objekta 'Stripe' u 'appsettings.json' fajlu
		public string PublishableKey { get; set; }//Ovaj properti mora da ima isti naziv kao properti JSON objekta 'Stripe' u 'appsettings.json' fajlu
	}
}
