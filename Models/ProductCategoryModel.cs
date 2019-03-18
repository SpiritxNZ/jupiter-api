using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jupiter.Models
{
    public class ProductCategoryModel
    {
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}