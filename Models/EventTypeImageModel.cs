using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace jupiterCore.Models
{
    public class EventTypeImageModel
    {
        public string Id { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
