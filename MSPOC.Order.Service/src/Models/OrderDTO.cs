using System;
using System.Collections;
using System.Collections.Generic;

namespace MSPOC.Order.Service.Models
{
    public record OrderDTO
    (
        Guid Id, 
        string Number, 
        string Description, 
        DateTimeOffset DeliveryDate, 
        CustomerDTO Customer, 
        IEnumerable<OrderItemDTO> OrderItems
    );

    public record CreateEditOrderDTO
    (
        string Number, 
        string Description, 
        DateTimeOffset DeliveryDate, 
        Guid CustomerId,
        IEnumerable<CreateEditOrderItemDTO> OrderItems
    );
}