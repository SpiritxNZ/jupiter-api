using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class ProductMedia
    {
        public int Id { get; set; }
        public int? ProdId { get; set; }
        public string Url { get; set; }

        public virtual Product Prod { get; set; }
    }
}
