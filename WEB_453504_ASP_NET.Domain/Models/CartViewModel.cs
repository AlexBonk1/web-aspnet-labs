using System;
using System.Collections.Generic;
using System.Text;

namespace WEB_453504_ASP_NET.Domain.Models
{
    public class CartViewModel
    {
        public List<WEB_453504_ASP_NET.Domain.Entities.MusicalInstrument> instruments { get; set; }
        public List<WEB_453504_ASP_NET.Domain.Entities.Category> categories { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

    }
}
