using WEB_453504_ASP_NET.Domain.Entities;

namespace WEB_453504_ASP_NET_Domain.Entities;


public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public MusicalInstrument Product { get; set; } = null!;
    public int Count { get; set; }
}