using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jupiterCore.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "Email address is Required.")]
        [EmailAddress(ErrorMessage = "Email looks fake or invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "The {0} does not meet requirements.")]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }
        [Range(0, 1)]
        [Column(TypeName = "decimal(3,2)")]
        public decimal Discount { get; set; }

        public DateTime CreatedOn { get; set; }

        public byte IsSubscribe { get; set; }

        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ComfirmPassword { get; set; }
    }
}
