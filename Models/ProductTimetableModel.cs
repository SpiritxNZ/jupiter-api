using System;

namespace jupiterCore.Models
{
    public class ProductTimetableModel
    {
        public int Id { get; set; }
        public int? ProdDetailId { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Quantity { get; set; }

        //public ProductDetailModel ProdDetail { get; set; }
    }
}
