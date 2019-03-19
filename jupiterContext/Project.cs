using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class Project
    {
        public Project()
        {
            ProjectMedia = new HashSet<ProjectMedia>();
        }

        public int ProdjectId { get; set; }
        public string Description { get; set; }
        public int? EventtypeId { get; set; }
        public string CustomerName { get; set; }

        public virtual EventType Eventtype { get; set; }
        public virtual ICollection<ProjectMedia> ProjectMedia { get; set; }
    }
}
