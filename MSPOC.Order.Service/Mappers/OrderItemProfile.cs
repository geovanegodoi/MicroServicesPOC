using System;
using AutoMapper;
using MSPOC.Events.Order;
using MSPOC.Order.Service.Models;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Mappers
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<OrderItemDTO, Entity.OrderItem>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(GetDate<OrderItemDTO>()))
                .ReverseMap();

            CreateMap<CreateEditOrderItemDTO, Entity.OrderItem>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(GetDate<CreateEditOrderItemDTO>()))
                .ReverseMap();

            CreateMap<Entity.OrderItem, OrderItemEvent>()
                .ConstructUsing(src => new OrderItemEvent(src.Id, src.Quantity));
        }

        private static Func<T, Entity.OrderItem, DateTimeOffset> GetDate<T>()
            => (src, dest) => 
                dest.CreatedDate == DateTimeOffset.MinValue ? 
                DateTimeOffset.Now : 
                dest.CreatedDate;
    }
}