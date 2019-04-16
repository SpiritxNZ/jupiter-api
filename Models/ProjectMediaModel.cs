using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace jupiterCore.Models
{
    public class ProjectMediaModel
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string Url { get; set; }
        public IFormFile file { get; set; }
    }
}
