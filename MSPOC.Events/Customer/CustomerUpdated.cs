using System;

namespace MSPOC.Events.Customer
{
    public record CustomerUpdated(Guid CustomerId, string Name, string Email);
}
