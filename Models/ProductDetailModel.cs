using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class ProductDetailModel
    {
        public int? Id { get; set; }
        [Required (ErrorMessage = "Product Id is required.")]
        public int ProdId { get; set; }
        public string ProductDetail1 { get; set; }
        public int? TotalStock { get; set; }
        public int? AvailableStock { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
    }
}
