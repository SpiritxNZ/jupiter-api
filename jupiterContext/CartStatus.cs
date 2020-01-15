using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public class CartStatus
    {
        public int CartStatusId { get; set; }
        public string CartStatusName { get; set; }

        public virtual ICollection<Cart> Cart { get; set; }
    }
}
