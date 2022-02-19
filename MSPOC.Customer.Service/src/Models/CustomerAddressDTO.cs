using System;

namespace MSPOC.Customer.Service.Models
{
    public record CustomerAddressDTO(string Street, string Number, string PostalCode, string City);

    public record CreateEditCustomerAddressDTO(string Street, string Number, string PostalCode, string City);
}