using Microsoft.AspNetCore.Mvc;

namespace WEB_453504_ASP_NET.UI.Controllers
{
    public class Product : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
