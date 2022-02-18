using MSPOC.CrossCutting.Mappers;
using MSPOC.Events.Order;
using MSPOC.Order.Service.Mappers.Extensions;
using MSPOC.Order.Service.Models;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Mappers
{
    public class OrderProfile : ProfileBaseWithEvents<Entity.Order, OrderDTO, CreateEditOrderDTO> 
    {
        protected override void ConfigureEventsMapping()
        {
            CreateMap<Entity.Order, OrderCreated>()
                .ConstructUsing(src => src.AsOrderCreated());

            CreateMap<Entity.Order, OrderUpdated>()
                .ConstructUsing(src => src.AsOrderUpdated());

            CreateMap<Entity.Order, OrderRemoved>()
                .ConstructUsing(src => src.AsOrderRemoved());
        }
    }
}