using System;

namespace MSPOC.Customer.Service.Models
{
    public record OrderHistoryDTO(Guid OrderId, string Description, DateTimeOffset OrderedDate, DateTimeOffset DeliveryDate);
}