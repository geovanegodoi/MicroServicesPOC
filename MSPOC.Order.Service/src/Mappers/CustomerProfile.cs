using System;
using AutoMapper;
using MSPOC.Events.Customer;
using MSPOC.Order.Service.Models;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Mappers
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerCreated, Entity.Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.CustomerId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));
        
            CreateMap<CustomerUpdated, Entity.Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.CustomerId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));
            
            CreateMap<CustomerDeleted, Entity.Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.CustomerId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));

            CreateMap<Entity.Customer, CustomerDTO>()
                .ReverseMap();
        }
    }
}