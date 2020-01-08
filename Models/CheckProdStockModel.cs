using System;
namespace jupiterCore.Models
{
    public class CheckProdStockModel
    {
        public int? proddetailid { get; set; }
        public int quantity { get; set; }
        public DateTime beginDate { get; set; }
        public int? prodid { get; set; } 

    }
}
