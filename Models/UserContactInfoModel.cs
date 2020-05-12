using System;
using jupiterCore.jupiterContext;

namespace jupiterCore.Models
{
    public class UserContactInfoModel
    {
        public int UserContactId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public byte IsSubscribe { get; set; }
        public string Comments { get; set; }
        public decimal Discount { get; set; }
        public string Website { get; set; }
        public string SocialMedia { get; set; }
        public string BusinessInfo { get; set; }
        public string Nzbn { get; set; }

        public User User { get; set; }
    }
}
