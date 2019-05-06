using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class StudentModel
    {
        [Required (ErrorMessage = "Student Id is required")]
        public int? Id { get; set; }
        [Required (ErrorMessage = "Please input student name.")]
        public string StudentName { get; set; }
    }
}
