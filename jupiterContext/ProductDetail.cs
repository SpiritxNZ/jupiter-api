using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class ProductDetail
    {
        public int Id { get; set; }
        public int ProdId { get; set; }
        public string ProductDetail1 { get; set; }
        public int? TotalStock { get; set; }
        public int? AvailableStock { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }

        public virtual Product Prod { get; set; }
    }
}
