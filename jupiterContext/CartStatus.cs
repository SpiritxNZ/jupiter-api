using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace jupiterCore.jupiterContext
{
    public class CartStatus
    {
        public int CartStatusId { get; set; }
        public string CartStatusName { get; set; }
        [JsonIgnore]
        public virtual ICollection<Cart> Cart { get; set; }
    }
}
