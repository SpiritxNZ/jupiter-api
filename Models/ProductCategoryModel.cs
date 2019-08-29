using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jupiter.Models
{
    public class ProductCategoryModel
    {
        public int? CategoryId { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Product type is required")]
        public int ProdTypeId { get; set; }
    }
}