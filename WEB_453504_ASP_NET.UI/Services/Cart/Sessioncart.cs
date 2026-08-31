using System.Security.Claims;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.UI.Extensions;

namespace WEB_453504_ASP_NET.UI.Services.Cart;



public class SessionCart : Domain.Entities.Cart
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SessionCart> _logger;

    Domain.Entities.Cart _cart;
    public SessionCart(IHttpContextAccessor httpContextAccessor, ILogger<SessionCart> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        var context = _httpContextAccessor.HttpContext;
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        _cart = context.Session.Get<Domain.Entities.Cart>($"cart_{userId}") ?? new();
        _logger.LogInformation($"Корзина пользователя {userId}.");

    }

    public override void AddToCart(MusicalInstrument instrument)
    {
        _cart.AddToCart(instrument);
        SaveCart();
    }

    public override void RemoveItems(int id)
    {
        _cart.RemoveItems(id);
        SaveCart();
    }

    public override void ClearAll()
    {
        _cart.ClearAll();
        SaveCart();
    }

    private void SaveCart()
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        _httpContextAccessor.HttpContext.Session.Set($"cart_{userId}", _cart);
    }
}