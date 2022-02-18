using System;

namespace MSPOC.Events.Order
{
    public record OrderItemEvent(Guid ItemId, int Quantity);
}
