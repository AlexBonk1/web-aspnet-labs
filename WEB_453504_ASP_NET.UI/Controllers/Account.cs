using Microsoft.AspNetCore.Mvc;

namespace WEB_453504_ASP_NET.UI.Controllers
{
    public class Account : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogOut()
        {
            return View();
        }
    }
}
