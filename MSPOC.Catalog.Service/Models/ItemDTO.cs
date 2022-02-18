using System;

namespace MSPOC.Catalog.Service.Models
{
    public record ItemDTO(Guid Id, string Name, string Description, decimal Price, int Quantity, DateTimeOffset CreatedDate);

    public record CreateEditItemDTO(string Name, string Description, decimal Price, int Quantity);
}