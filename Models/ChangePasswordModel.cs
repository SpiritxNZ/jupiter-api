using System;
using System.ComponentModel.DataAnnotations;

namespace jupiterCore.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "email is required")]
        public string email { get; set; }
        [Required(ErrorMessage = "oldPassword is required")]
        public string oldPassword { get; set; }
        [Required(ErrorMessage = "newPassword is required")]
        public string newPassword { get; set; }
    }
}
