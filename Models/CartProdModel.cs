using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jupiter.Models
{
    public class CartProdModel
    {
        public int? Id { get; set; }
        public int? CartId { get; set; }
        public int? ProdId { get; set; }
        public decimal? Price { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int? Quantity { get; set; }
    }
}