using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using jupiterCore.jupiterContext;

namespace Jupiter.Models
{
    public class CartModel
    {
        public int? CartId { get; set; }
        public decimal? Price { get; set; }
        [Required (ErrorMessage ="Location is Required.")]
        public string Location { get; set; }
        public DateTime PlannedTime { get; set; }
        [Range(0, 1, ErrorMessage = "IsActivate must be either 0 or 1")]
        public byte? IsActivate { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int? ContactId { get; set; }

        
        public Contact Contact { get; set; }
        public IEnumerable<CartProd> CartProd { get; set; }
    }
}