using Microsoft.AspNetCore.Mvc;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.UI.Extensions;
using WEB_453504_ASP_NET.UI.Models;
using System.Security.Claims;


namespace WEB_453504_ASP_NET.UI.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var cart = HttpContext.Session.Get<Cart>($"cart_{userId}") ?? new();
            CartInfo info = new CartInfo()
            {
                Price = 67,
                Items = 10
            };
            if(cart != null)
            {
                info.Price = cart.TotalPrice;
                info.Items = cart.Count;
            }

            return View(info);
        }
    }
}
