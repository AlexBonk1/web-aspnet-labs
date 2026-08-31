using System.Collections;
using WEB_453504_ASP_NET_Domain.Entities;

namespace WEB_453504_ASP_NET.Domain.Entities;


public class Cart
{
    public int Id { get; set; }
    private Dictionary<int, CartItem> _cartItems = new Dictionary<int, CartItem>();
    public Dictionary<int,CartItem> CartItems { get => _cartItems; set => _cartItems = value; }
    public virtual void AddToCart(MusicalInstrument instrument)
    {
        if (CartItems.ContainsKey(instrument.Id))
        {
            CartItems[instrument.Id].Count++;
        }
        else
        {
            CartItems.Add(instrument.Id, new CartItem { Product = instrument, Count = 1 });
        }
    }

    /// <summary>
    /// Удалить объект из корзины
    /// </summary>
    /// <param name="id"> id удаляемого объекта</param>
    public virtual void RemoveItems(int id)
    {
        CartItems.Remove(id);
    }


    /// <summary>
    /// Очистить корзину
    /// </summary>
    public virtual void ClearAll()
    {
        CartItems.Clear();
    }

    /// <summary>
    /// Количество объектов в корзине
    /// </summary>
    public int Count { get => CartItems.Sum(item => item.Value.Count); }
    /// <summary>
    /// Общая стоимость объектов в корзине
    /// </summary>
    public decimal TotalPrice { get=> CartItems.Sum(item=> item.Value.Product.Price*item.Value.Count); }

}