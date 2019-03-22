using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jupiter.Models;

namespace jupiterCore.Models
{
    public class CartContactModel
    {
        public CartModel CartModel { get; set; }
        public ContactModel ContactModel { get; set; }
    }
}
