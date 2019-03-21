using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class ProjectMediaModel
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string Url { get; set; }
    }
}
