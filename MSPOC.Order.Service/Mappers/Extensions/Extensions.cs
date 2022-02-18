using System.Collections.Generic;
using System.Linq;
using MSPOC.Events.Order;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Mappers.Extensions
{
    public static class Extensions
    {
        public static OrderCreated AsOrderCreated(this Entity.Order order)
            => new OrderCreated
            (
                order.Id,
                order.Customer.Id,
                order.Description,
                order.CalculateOrderTotalPrice(),
                order.CreatedDate,
                order.DeliveryDate,
                order.OrderItems.AsOrderItemEventCollection()
            );

        public static OrderUpdated AsOrderUpdated(this Entity.Order order)
            => new OrderUpdated
            (
                order.Id,
                order.Customer.Id,
                order.Description,
                order.CalculateOrderTotalPrice(),
                order.CreatedDate,
                order.DeliveryDate,
                order.OrderItems.AsOrderItemEventCollection()
            );

        public static OrderRemoved AsOrderRemoved(this Entity.Order order)
            => new OrderRemoved
            (
                order.Id, 
                order.Customer.Id, 
                order.OrderItems.AsOrderItemEventCollection()
            );

        public static IEnumerable<OrderItemEvent> AsOrderItemEventCollection(this IEnumerable<Entity.OrderItem> orderItems)
            => orderItems.Select(i => new OrderItemEvent(i.Id, i.Quantity));
    }
}