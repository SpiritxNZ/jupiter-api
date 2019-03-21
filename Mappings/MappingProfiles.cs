using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Jupiter.Models;

namespace jupiterCore.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Cart, CartModel>().ReverseMap();
            CreateMap<CartProd, CartProdModel>().ReverseMap();
            CreateMap<Contact, ContactModel>().ReverseMap();
            CreateMap<EventTypeModel, EventType>();
            CreateMap<ProductModel, Product>();
            CreateMap<ProductCategoryModel, ProductCategory>();
            CreateMap<ProductMediaModel, ProductMedia>();
            CreateMap<ProductTypeModel, ProductType>();
            CreateMap<TestimonialModel, Testimonial>();
            CreateMap<ProjectModel, Project>();
            CreateMap<ProjectMediaModel, ProductMedia>();
        }
    }
}
