using Microsoft.AspNetCore.Mvc;
using WEB_453504_ASP_NET.Domain.Models;
using WEB_453504_ASP_NET.UI.Extensions;
using WEB_453504_ASP_NET.UI.Services.CategoryService;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;

namespace WEB_453504_ASP_NET.UI.Controllers
{

    [Route("Catalog")]
    public class Product(IInstrumentService _service, ICategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            var productResponse = await _service.GetInstrumentListAsync(category, pageNo);
            var resp = await categoryService.GetCategoryListAsync();
            if (!productResponse.Successfull)
                return NotFound(productResponse.ErrorMessage);
            if (!resp.Successfull)
                return NotFound(productResponse.ErrorMessage);
            var cat = resp.Data.Find(c => c.NormalizedName == category);
            if (cat == null || category == null)
            {
                ViewData["currentCategory"] = "Все";
            }
            else
            {
                ViewData["currentCategory"] = cat.Name;
            }

            ViewBag.CurrentPage = pageNo;


            var vm = new CartViewModel
            {
                instruments = productResponse.Data.Items,
                categories = resp.Data,
                TotalPages = productResponse.Data.TotalPages,
                CurrentPage = productResponse.Data.CurrentPage
            };

            if (Request.IsAjaxRequest())
                return PartialView("_ProductListPartial", vm);

            return View(vm);
        }
    }
}
