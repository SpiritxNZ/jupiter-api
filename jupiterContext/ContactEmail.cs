using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class ContactEmail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public DateTime? DateOfEvent { get; set; }
        public string LocationOfEvent { get; set; }
        public string FindUs { get; set; }
        public string TypeOfEvent { get; set; }
        public string Message { get; set; }
    }
}
