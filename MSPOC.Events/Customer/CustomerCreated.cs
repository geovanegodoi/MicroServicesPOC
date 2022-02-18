using System;

namespace MSPOC.Events.Customer
{
    public record CustomerCreated(Guid CustomerId, string Name, string Email);
}