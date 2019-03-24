using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class ContactEmailModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public DateTime DateOfEvent { get; set; }
        public string Location { get; set; }
        public string FindUs { get; set; }
        public string TypeOfEvent { get; set; }
        public string Message { get; set; }
    }
}
