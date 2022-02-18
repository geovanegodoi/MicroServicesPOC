using System;
using System.Collections.Generic;

namespace MSPOC.Events.Order
{
    public record OrderCreated
    (
        Guid OrderId, 
        Guid CustomerId, 
        string Description, 
        decimal TotalPrice, 
        DateTimeOffset OrderedDate, 
        DateTimeOffset DeliveryDate, 
        IEnumerable<OrderItemEvent> OrderItems
    );
}
