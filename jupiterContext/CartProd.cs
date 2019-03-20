using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class CartProd
    {
        public int Id { get; set; }
        public int? CartId { get; set; }
        public int? ProdId { get; set; }
        public decimal? Price { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int? Quantity { get; set; }

        
        public virtual Cart Cart { get; set; }
        public virtual Product Prod { get; set; }
    }
}
