using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

//for validation



namespace Jupiter.Models
{
    public class ProductModel
    {
        public int? ProdId { get; set; }
        public int? ProdTypeId { get; set; }
        public int? CategoryId { get; set; }
        public string ProdTypeName { get; set; }
        public string CategoryName { get; set; }
        //[Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        //[Required(ErrorMessage = "SubTitle is required")]
        public string SubTitle { get; set; }
        //[Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public int? TotalStock { get; set; }
        public int? AvailableStock { get; set; }
        public decimal? Price { get; set; }
        [Range(0, 1, ErrorMessage = "SpcOrDisct must be a number between 0 and 1")]
        public short? SpcOrDisct { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SpecialPrice { get; set; }
        [Range(0, 1, ErrorMessage = "IsActivate must be a number between 0 and 1")]
        public byte? IsActivate { get; set; }
        public DateTime? CreateOn { get; set; }
        public byte? SpecialOrder { get; set; }

        public IEnumerable<string> ProdMedias { get; set; }
        public IFormFile ProductImage { get; set; }
    }
}