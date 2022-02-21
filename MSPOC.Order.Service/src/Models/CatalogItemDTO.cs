using System;

namespace MSPOC.Order.Service.Models
{
    public record CatalogItemDTO(Guid Id, string Name, decimal Price);
}