using System;
using System.ComponentModel.DataAnnotations;

namespace jupiterCore.Models
{
    public class ResetPasswordModel
    {
        public string token { get; set; }
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
