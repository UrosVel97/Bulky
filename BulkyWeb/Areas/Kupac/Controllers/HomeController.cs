using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Kupac.Controllers
{
    //Moramo da naglasimo u koj 'Area' se nalazi kontroler
    [Area("Kupac")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
                
        }

        public IActionResult Index()
        {

            List<Product> productList = _unitOfWork.Proizvod.GetAll(includeProperties: "Category").ToList();

            return View(productList);

        }


        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Proizvod.Get(u => u.Id == id, includeProperties: "Category"),
                Count = 1,
                ProductId = id
            };


            return View(cart);
        }

        [HttpPost]
        [Authorize]/*Ova anotacija podataka 'Authorize' kaze da korisnik ce moci da pozove ovu 
                    * metodu 'Details()' samo ako je registrovani korisnik, ako je posetioc onda 
                    * nece moci da doda u korpu artikle*/
        public IActionResult Details(ShoppingCart cart)
        {
            cart.Id = 0; //Postoji Bug koji automatksi dodeljuje propertiju 'Id' vrednost kada se vraca iz pogleda

            /*Kada registrovani korisnik klikne na dugme dodaj u korpu,
             * mi hocemo da uzmemo 'UserId' tog korisnika i da sacuvamo u 
             * property 'ApplicationUserId' klase 'ShoppingCart'. */
                
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            cart.ApplicationUserId = userId;

            ShoppingCart cartFromDb=_unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId==userId && u.ProductId==cart.ProductId);

            if (cartFromDb != null)
            {
                //Postoji vec korpa u bazi
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();


                //Saljemo kljuc i vrednost pretrazivacu koji ce da smesti ta dva podatka u kolacic, i njima ce moci da se pristupi sa klijentske strane
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            else
            {
                //Kreiraj novu korpu u bazi
                _unitOfWork.ShoppingCart.Add(cart); 
                _unitOfWork.Save();
                //Saljemo kljuc i vrednost pretrazivacu koji ce da smesti ta dva podatka u kolacic, i njima ce moci da se pristupi sa klijentske strane
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            }



            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
