using AutoMapper;
using Ecom.Dto;
using Ecom.Models;
using System.Diagnostics.Metrics;

namespace Ecom.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();
            CreateMap<UserRegister,  UserRegisterDto>();
            CreateMap<UserRegisterDto, UserRegister>();
            
        }
    }
}
