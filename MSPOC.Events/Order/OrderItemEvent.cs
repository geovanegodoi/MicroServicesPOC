using System;

namespace MSPOC.Events.Order
{
    public record OrderItemEvent(Guid ItemId, int Quantity);

    public record OrderItemUpdatedEvent(Guid ItemId, int OldQuantity, int NewQuantity);
}
