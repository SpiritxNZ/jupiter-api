using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class EventType
    {
        public EventType()
        {
            Project = new HashSet<Project>();
            Testimonial = new HashSet<Testimonial>();
        }

        public int TypeId { get; set; }
        public string EventName { get; set; }

        public virtual ICollection<Project> Project { get; set; }
        public virtual ICollection<Testimonial> Testimonial { get; set; }
    }
}
