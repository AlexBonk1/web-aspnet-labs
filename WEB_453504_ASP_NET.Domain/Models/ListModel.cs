using System;
using System.Collections.Generic;
using System.Text;

namespace WEB_453504_ASP_NET.Domain.Models
{
    public class ListModel<T>
    {
        // запрошенный список объектов
        public List<T> Items { get; set; } = new();
        // номер текущей страницы
        public int CurrentPage { get; set; } = 1;
        // общее количество страниц
        public int TotalPages { get; set; } = 1;
    }
}
