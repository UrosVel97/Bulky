using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace BulkyWeb.Areas.Kupac.Controllers
{
    [Area("Kupac")]
    [Authorize]//Samo korisnici koji su registrovani i prijavljeni na sajt ce moci da koriste metode kontrolera 'CartController'
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

		/*Naglasavamo da properti 'ShoppingCartVM' ce da dobije vrednost za svoje atribute kada se vraca iz pogleda. 
		 * Radi isto kao kod Razor Pages projekta.*/
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        private readonly IEmailSender _emailSender; //Ova promenljiva koja je tipa interfejsa 'IEmailSender' ce biti zaduzena za slanje mejlova

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }


        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Vracamo iz baze sve artikle koje je korisnik sa odredjenim 'userId' stavio u korpu
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader=new()
            };

            //Sledeca foreach petlja je da izracunamo ukupnu cenu svih artikla u korpi
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }


            return View(ShoppingCartVM);
        }

        //Ova Action metoda prikazuje 'Summary' pogled
        public IActionResult Summary()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Vracamo iz baze sve artikle koje je korisnik sa odredjenim 'userId' stavio u korpu
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            //Sledeca foreach petlja je da izracunamo ukupnu cenu svih artikla u korpi
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]//Eksplicitno naglasavamo da se ova metoda ustvari zove 'Summary'. Kada hocemo da je pozovemo preko URL-a pisemo 'Summary' a ne 'SummaryPOST'
		public IActionResult SummaryPOST()
		{

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Vracamo iz baze sve artikle koje je korisnik sa odredjenim 'userId' stavio u korpu
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
            
            ShoppingCartVM.OrderHeader.OrderDate=System.DateTime.Now; //Datum porucivanja
            ShoppingCartVM.OrderHeader.ApplicationUserId= userId;


			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


				//Sledeca foreach petlja je da izracunamo ukupnu cenu svih artikla u korpi
				foreach (var cart in ShoppingCartVM.ShoppingCartList)
			    {
				    cart.Price = GetPriceBasedOnQuantity(cart);
				    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			    }

			//Proveravamo da li korisnik ima ulogu 'Company'. Ako ima onda drugacije postupamo sa njim u vezi placanja
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)//Metoda 'GetValueOrDefault()' radi sa 'null' vrednostima i ne izbacuje gresku ako je vrednost null
			{
                //U pitanju je regularni korisnik 
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
            else
            {
				//U pitanju je korisnik koji je kompanija i on ce imati rok od 30 dana za placanje
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayement;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader); //Dodajemo u tabeli 'OrderHeaders' novi slog
            _unitOfWork.Save();

			//Kreiramo objekte tipa 'OrederDetail', ima ce ih onoliko koliko ima elemenata lista 'ShoppingCartList'
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            
            //Ako je u pitanju obican korisnik, onda moramo da ga preusmerimo na 'Stripe' stranicu za placanje
            if(applicationUser.CompanyId.GetValueOrDefault()==0)
            {
                var domain = "https://localhost:7279/"; //Ovo je domen na lokalnoj masini, to ce morati da se promeni kada se deploy-uje

				var options = new Stripe.Checkout.SessionCreateOptions //Opcije za kreiranje sesije
				{
					SuccessUrl = domain + $"kupac/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}", //Kada se uspesno izvrsi placanje, 'Stripe' ce da preusmeri korisnika na Action Metodu 'OrderConfirmation()'
					CancelUrl = domain + "customer/cart/index", //Kada korisnik klikne na dugme 'Vrati se nazad' prebacice ga na ovai pogled 'Index.cshtml'
					LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(), //Ovo je lista artikla koje je kupac narucio i koji ce se prikazati na 'Stripe' stranici
					Mode = "payment", //Mod sesije
				};
                foreach(var item in ShoppingCartVM.ShoppingCartList)
                {
					//Dodajemo artikle u listu 'LineItems' tako da se pokazu na 'Stripe stranici'
					var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), //20.50$ => 2050$ . Cene se konvertuju u long format
                            Currency = "usd", //Valuta koja ce se prikazati na 'Stripe stranici'
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title, //Naziv artikla koji ce se prikazati na 'Stripe' stranici
								Description = item.Product.Description //Opis artikla koji ce se prikazati na 'Stripe' stranici

                                
                                
                            }

                        },
                        Quantity = item.Count //Broj artikla koji ce se prikazati na stripe stranici
                    };
                    options.LineItems.Add(sessionLineItem);

                }

				var service = new Stripe.Checkout.SessionService(); //Kreiramo novi Servis Sesije
				Session session=service.Create(options); //Kreiramo sesiju pomocu objekta tipa 'SessionService' i dodeljujemo opcije koje smo naveli preko objekta tia 'SessionCreateOptions'
				

                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);//Posto smo kreirali sesiju onda dodeljujemo propertiju 'SessionId' klase 'OrderHeader', id kreirane sesije. Properti 'session.PaymentIntentId' ce biti null u ovoj situaciji jer jos nije uspesno izvrseno placanje
				_unitOfWork.Save();
                
                Response.Headers.Add("Location", session.Url); //Preusmeravamo korisnika na 'Stripe' stranicu za placanje
                return new StatusCodeResult(303); //Ovaj status kod '303' kaze da metoda je preusmerila korisnika na neku drugu stranicu 

			}

            //Ako je u pitanju firma, onda preusmeravamo korisnika na pogled 'OrderConfirmation.cshtml'
			return RedirectToAction(nameof(OrderConfirmation), new {id=ShoppingCartVM.OrderHeader.Id}) ;
		}


        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if(orderHeader.PaymentStatus!=SD.PaymentStatusDelayedPayement)
            {
                //Ovu porudzbinu je napravio obican korisnik a ne firma
                var service = new SessionService();
                Session session=service.Get(orderHeader.SessionId);

                if(session.PaymentStatus.ToLower()=="paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id,session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id,SD.StatusApproved,SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.SetInt32(SD.SessionCart,0);

            }

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book",
                $"<p>New Order Created - {orderHeader.Id}</p>");
            //Brisemo sve 'ShoppingCart' modele koje je korisnik napravio
            List<ShoppingCart> shoppingCarts=_unitOfWork.ShoppingCart.
                GetAll(u=>u.ApplicationUserId==orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);
        }

		//Action metoda za inkrementiranje broja artikla u korpi
		public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }

        //Action metoda za dekrementiranje broja artikla u korpi
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId,tracked:true);
            if (cartFromDb.Count <= 1)
            {

                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                        .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
                //Brisemo iz korpe artikal
                _unitOfWork.ShoppingCart.Remove(cartFromDb);

            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }


        //Action metoda za brisanje artikla iz korpe
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId,tracked:true);

            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            //Brisemo iz korpe artikal
            _unitOfWork.ShoppingCart.Remove(cartFromDb);

            _unitOfWork.Save();

           
            return RedirectToAction(nameof(Index));

        }



        //Funkcija koja vraca cenu svakog proizvoda na osnovu kolicine
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }


        }
    }
}
