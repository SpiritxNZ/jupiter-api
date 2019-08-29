using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace jupiterCore.Models
{
    public class HomepageCarouselMediaModel
    {
        public int? Id { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile file { get; set; }
    }
}
