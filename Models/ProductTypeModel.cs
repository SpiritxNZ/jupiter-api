using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace jupiterCore.Models
{
    public class ProductTypeModel
    {
        public int ProdTypeId { get; set; }
        [Required(ErrorMessage = "Typename is required")]
        public string TypeName { get; set; }
    }
}
