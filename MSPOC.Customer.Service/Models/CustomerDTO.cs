using System;

namespace MSPOC.Customer.Service.Models
{
    public record CustomerDTO
    (
        Guid Id, 
        string Name, 
        string Email,
        string DocumentNumber,
        string PhoneNumber,
        string CellNumber,
        CustomerAddressDTO Address
    );

    public record CreateEditCustomerDTO
    (
        string Name, 
        string Email,
        string DocumentNumber,
        string PhoneNumber,
        string CellNumber,
        CreateEditCustomerAddressDTO Address
    );
}