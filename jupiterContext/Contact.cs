using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class Contact
    {
        public Contact()
        {
            Cart = new HashSet<Cart>();
        }

        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNum { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string FindUs { get; set; }

        public virtual ICollection<Cart> Cart { get; set; }
    }
}
