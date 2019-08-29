using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        public int? ProdTypeId { get; set; }

        [JsonIgnore]
        public virtual ProductType ProdType { get; set; }
        [JsonIgnore]
        public virtual ICollection<Product> Product { get; set; }
    }
}
