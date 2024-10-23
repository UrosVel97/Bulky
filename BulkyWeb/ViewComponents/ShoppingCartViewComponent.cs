using Bulky.DataAccess.Reporsitory;
using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unit;

        public ShoppingCartViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionCart)==null)
                {
                    //Saljemo kljuc i vrednost pretrazivacu koji ce da smesti ta dva podatka u kolacic, i njima ce moci da se pristupi sa klijentske strane
                    HttpContext.Session.SetInt32(SD.SessionCart, 
                        _unit.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }


                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }

    }
}
