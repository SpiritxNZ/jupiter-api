using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class ClassModel
    {
        [Required (ErrorMessage = "Class Id should not be null.")]
        public int Id { get; set; }
        [Required (ErrorMessage = "Class name required.")]
        public string ClassName { get; set; }
        public ICollection<StudentModel> StudentModel { get; set; }
    }
}
