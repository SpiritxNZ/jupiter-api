using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace jupiterCore.jupiterContext
{
    public partial class ProductType
    {
        public ProductType()
        {
            Product = new HashSet<Product>();
            ProductCategory = new HashSet<ProductCategory>();
        }

        public int ProdTypeId { get; set; }
        public string TypeName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<ProductCategory> ProductCategory { get; set; }
    }
}
