using System;
using AutoMapper;
using MSPOC.Customer.Service.Entities;
using MSPOC.Events.Order;

namespace MSPOC.Customer.Service.Mappers
{
    public class CustomerOrderHistoryProfile : Profile
    {
        public CustomerOrderHistoryProfile()
        {
            CreateMap<OrderCreated, OrderHistory>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));

            CreateMap<OrderUpdated, OrderHistory>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));

            CreateMap<OrderRemoved, OrderHistory>();
        }
    }
}