﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace jupiterCore.jupiterContext
{
    public partial class UserContactInfo
    {
        public int UserContactId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
