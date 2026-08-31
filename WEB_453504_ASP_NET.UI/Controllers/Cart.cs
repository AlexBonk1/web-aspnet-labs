using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.UI.Extensions;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;
using WEB_453504_ASP_NET_Domain.Entities;
using System.Security.Claims;


namespace WEB_453504_ASP_NET.UI.Controllers
{
    public class CartController : Controller
    {
        Cart _cart;
        IInstrumentService _instrumentService;
        ILogger<CartController> _logger;
        public CartController(IInstrumentService instrumentService, ILogger<CartController> logger, Cart cart)
        {
            _instrumentService = instrumentService;
            _logger = logger;
            _cart = cart;
        }
        public IActionResult Index()
        {
            Cart cart = HttpContext.Session.Get<Cart>($"cart_{User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value}") ?? new();
            return View(cart);
        }

        [Authorize]
        [Route("[controller]/add/{id:int}")]
        public async Task<ActionResult> Add(int id, string returnUrl)
        {

        var data = await _instrumentService.GetInstrumentByIdAsync(id);
        if (data.Successfull && data.Data != null)
        {
            _cart.AddToCart(data.Data);
            _logger.LogInformation($"Корзина обновлена. {_cart.Count} товаров на сумму {_cart.TotalPrice}");
        }
        return Redirect(returnUrl);
        }

    
        [HttpPost]
        [Route("[controller]/delete/{id:int}")]

        public ActionResult Delete(int id,string returnUrl)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            _cart.RemoveItems(id);
            _logger.LogInformation($"Корзина обновлена. {_cart.Count} товаров на сумму {_cart.TotalPrice}");
            return Redirect(returnUrl);
        }
    }
}
