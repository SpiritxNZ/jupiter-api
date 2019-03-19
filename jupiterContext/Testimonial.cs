using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class Testimonial
    {
        public int TestimonialId { get; set; }
        public string CustomerName { get; set; }
        public int? EventtypeId { get; set; }
        public string Message { get; set; }

        public virtual EventType Eventtype { get; set; }
    }
}
