using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class ProductTimetable
    {
        public int Id { get; set; }
        public int? ProdId { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Quantity { get; set; }

        public virtual ProductDetail Prod { get; set; }
    }
}
