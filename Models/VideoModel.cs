using System;
using System.ComponentModel.DataAnnotations;

namespace jupiterCore.Models
{
    public class VideoModel
    {
        [Required]
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
