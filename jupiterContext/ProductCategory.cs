using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}
