using System;
using System.Collections.Generic;

namespace MSPOC.Order.Service.Models
{
    public record OrderItemDTO(Guid Id, string Name, decimal Price, int Quantity);

    public record CreateEditOrderItemDTO(Guid Id, int Quantity);
}