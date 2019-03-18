using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JupiterEntity;

namespace Jupiter.Models
{
    public class TestimonialModel
    {
        public int? TestimonialId { get; set; }
        public string CustomerName { get; set; }
        public int? EventtypeId { get; set; }
        public string Message { get; set; }
    }
}