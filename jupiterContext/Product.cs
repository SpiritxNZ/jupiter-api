using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class Product
    {
        public Product()
        {
            CartProd = new HashSet<CartProd>();
            ProductDetail = new HashSet<ProductDetail>();
            ProductMedia = new HashSet<ProductMedia>();
        }

        public int ProdId { get; set; }
        public int? ProdTypeId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int? TotalStock { get; set; }
        public int? AvailableStock { get; set; }
        public decimal? Price { get; set; }
        public short? SpcOrDisct { get; set; }
        public decimal? Discount { get; set; }
        public byte? IsActivate { get; set; }
        public DateTime? CreateOn { get; set; }

        public virtual ProductCategory Category { get; set; }
        public virtual ProductType ProdType { get; set; }
        public virtual ICollection<CartProd> CartProd { get; set; }
        public virtual ICollection<ProductDetail> ProductDetail { get; set; }
        public virtual ICollection<ProductMedia> ProductMedia { get; set; }
    }
}
