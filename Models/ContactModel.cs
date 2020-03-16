using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jupiter.Models
{
    public class ContactModel
    {
        public int? ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNum { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string FindUs { get; set; }
    }
}