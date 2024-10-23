using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string status)
        {
            List<OrderVM> OrderVMs;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                OrderVMs = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").
                Select(i => new OrderVM()
                {
                    OrderHeader = i,
                    OrderDetail = _unitOfWork.OrderDetail.
                GetAll(u => u.OrderHeaderId == i.Id)
                }).ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                
                OrderVMs = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId==userId,includeProperties: "ApplicationUser").
                Select(i => new OrderVM()
                {
                    OrderHeader = i,
                    OrderDetail = _unitOfWork.OrderDetail.
                GetAll(u => u.OrderHeaderId == i.Id)
                }).ToList();
            }

            switch (status)
            {
                case "pending":
                    OrderVMs = OrderVMs.Where(u => u.OrderHeader.OrderStatus == SD.StatusPending).ToList();
                    break;
                case "inprocess":
                    OrderVMs = OrderVMs.Where(u => u.OrderHeader.OrderStatus == SD.StatusInProcess).ToList();
                    break;
                case "completed":
                    OrderVMs = OrderVMs.Where(u => u.OrderHeader.OrderStatus == SD.StatusShipped).ToList();
                    break;
                case "approved":
                    OrderVMs = OrderVMs.Where(u => u.OrderHeader.OrderStatus == SD.StatusApproved).ToList();
                    break;
                default:

                    break;
            }

            return View(OrderVMs);
        }

        public IActionResult Details(int orderId)
        {
            this.OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.
                    GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)] //Samo korisnici koji su prijavljeni kao 'Admin' ili 'Employee' ce moci da azuriraju informacije o porudzbini
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb=_unitOfWork.OrderHeader.Get(u=>u.Id==OrderVM.OrderHeader.Id);

            orderHeaderFromDb.Name= OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            //U slucaju ako propertiji 'Carrier' i 'TrackingNumber' nisu null, onda cemo i njih da azuiriramo
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier= OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order Details Updated Successfully";



            return RedirectToAction(nameof(Details), new { orderId= orderHeaderFromDb.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            //Azuiriramo status porudzbine da ima vrednost 'StatusInProcess'
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();


            TempData["success"] = "Order Status Updated Successfully";
            return RedirectToAction(nameof(Details), new {orderId = OrderVM.OrderHeader.Id});

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {

            var orderHeader = _unitOfWork.OrderHeader.Get(u=>u.Id==OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber= OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier=OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate=DateTime.Now;

            /*U slucaju ako je korisnik sa ulogom 'Company' onda ce on imati rok
             * od 30 dana, od dana slanja, da plati*/
            if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayement)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            //Azuiriramo status porudzbine da ima vrednost 'StatusInProcess'
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();


            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            if(orderHeader.PaymentStatus== SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);

            }


            _unitOfWork.Save();


            TempData["success"] = "Order Canceled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader
                .Get(u=>u.Id == OrderVM.OrderHeader.Id, includeProperties:"ApplicationUser");
            OrderVM.OrderDetail=_unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId==OrderVM.OrderHeader.Id,includeProperties:"Product");

            var domain = Request.Scheme + "://" + Request.Host.Value + "/"; //Dinamicki pribavljamo URL. Nebitno da li je localhost ili Web

            var options = new Stripe.Checkout.SessionCreateOptions //Opcije za kreiranje sesije
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}", //Kada se uspesno izvrsi placanje, 'Stripe' ce da preusmeri korisnika na Action Metodu 'PaymentConfirmation()'
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}", //Kada korisnik klikne na dugme 'Vrati se nazad' prebacice ga na ovai pogled 'Details.cshtml'
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(), //Ovo je lista artikla koje je kupac narucio i koji ce se prikazati na 'Stripe' stranici
                Mode = "payment", //Mod sesije
            };
            foreach (var item in OrderVM.OrderDetail)
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
            Session session = service.Create(options); //Kreiramo sesiju pomocu objekta tipa 'SessionService' i dodeljujemo opcije koje smo naveli preko objekta tia 'SessionCreateOptions'


            _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);//Posto smo kreirali sesiju onda dodeljujemo propertiju 'SessionId' klase 'OrderHeader', id kreirane sesije. Properti 'session.PaymentIntentId' ce biti null u ovoj situaciji jer jos nije uspesno izvrseno placanje
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url); //Preusmeravamo korisnika na 'Stripe' stranicu za placanje
            return new StatusCodeResult(303); //Ovaj status kod '303' kaze da metoda je preusmerila korisnika na neku drugu stranicu 

        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayement)
            {
                //Ovu porudzbinu je napravio korisnik koji ima ulogu 'Company'
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

            }

            return View(orderHeaderId);
        }


    }
}
