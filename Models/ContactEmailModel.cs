using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class ContactEmailModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        [Required]
        public DateTime DateOfEvent { get; set; }
        [Required]
        public string LocationOfEvent { get; set; }
        [Required]
        public string FindUs { get; set; }
        [Required]
        public string TypeOfEvent { get; set; }
        [Required]
        public string Message { get; set; }
    }

    public class ContactUsModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string FindUs { get; set; }
    }
}
