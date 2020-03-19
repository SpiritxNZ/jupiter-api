using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace jupiterCore.jupiterContext
{
    public partial class User
    {
        public User()
        {
            UserContactInfo = new HashSet<UserContactInfo>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte IsSubscribe { get; set; }
        public DateTime CreatedOn { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal Discount { get; set; }

        public virtual ICollection<UserContactInfo> UserContactInfo { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
    }
}
