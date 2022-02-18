using System;
using System.Collections.Generic;

namespace MSPOC.Events.Order
{
    public record OrderRemoved
    (
        Guid OrderId, 
        Guid CustomerId, 
        IEnumerable<OrderItemEvent> OrderItems
    );
}
