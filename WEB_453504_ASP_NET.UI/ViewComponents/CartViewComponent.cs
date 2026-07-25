using Microsoft.AspNetCore.Mvc;
using WEB_453504_ASP_NET.UI.Models;

namespace WEB_453504_ASP_NET.UI.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            var info = new CartInfo()
            {
                Price = 67,
                Items = 10
            };

            return View(info);
        }
    }
}
