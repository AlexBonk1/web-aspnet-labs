using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using WEB_453504_ASP_NET.UI.Models;

namespace WEB_453504_ASP_NET.UI.Controllers
{
    public class Home : Controller
    {
        public IActionResult Index()
        {
            ViewData["LabName"] = "Лабораторная работа №2";

            List<ListDemo> listItems = new()
            {
                new() { Id = 1, Name = "Name1"},
                new() { Id = 2, Name = "Name2"},
                new() { Id = 3, Name = "Name3"},
                new() { Id = 4, Name = "Name4"},
                new() { Id = 5, Name = "Name5"},
            };
            ViewData["controller"] = "home";
            ViewData["MyList"] = new SelectList(listItems, "Id", "Name");


            return View();
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
