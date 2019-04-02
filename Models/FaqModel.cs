using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class FaqModel
    {
        public int? Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
