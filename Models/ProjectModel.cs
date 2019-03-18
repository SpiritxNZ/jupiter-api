using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jupiter.Models
{
    public class ProjectModel
    {
        public int? ProjectId { get; set; }
        public string Description { get; set; }
        public int? EventtypeId { get; set; }
        public string CustomerName { get; set; }
}
}