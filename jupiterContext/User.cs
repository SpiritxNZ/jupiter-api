using System;
using System.Collections.Generic;
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

        public virtual ICollection<UserContactInfo> UserContactInfo { get; set; }

        public static implicit operator string(User v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator User(Task<List<User>> v)
        {
            throw new NotImplementedException();
        }
    }
}
