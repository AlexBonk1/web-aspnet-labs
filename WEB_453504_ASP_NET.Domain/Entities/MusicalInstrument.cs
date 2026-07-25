using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace WEB_453504_ASP_NET.Domain.Entities
{
    public class MusicalInstrument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public double WeightKg { get; set; }

    }
}
