using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class UserContactInfo
    {
        public int UserContactId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }

        public virtual User User { get; set; }
    }
}
