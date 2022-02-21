using System;

namespace MSPOC.Order.Service.Models
{
    public record CustomerDTO(Guid Id, string Name, string Email);
}