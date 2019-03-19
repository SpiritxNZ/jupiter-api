using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class ProductType
    {
        public ProductType()
        {
            Product = new HashSet<Product>();
        }

        public int ProdTypeId { get; set; }
        public string TypeName { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}
