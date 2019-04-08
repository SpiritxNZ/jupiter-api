using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Jupiter.Models
{
    public class ProductMediaModel
    {
        public int? Id { get; set; }
        public string ProdId { get; set; }
        public string Url { get; set; }
        public IFormFile file { get; set; }
    }
}