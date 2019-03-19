using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class ProjectMedia
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string Url { get; set; }

        public virtual Project Project { get; set; }
    }
}
