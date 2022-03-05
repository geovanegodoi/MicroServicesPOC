using System;
using AutoMapper;
using Bogus;
using MSPOC.CrossCutting;
using MSPOC.Customer.Service.Models;
using MSPOC.Events.Customer;

namespace MSPOC.Customer.Service.UnitTest.Fixtures
{
    public class ControllerFixture
    {
        public IMapper MapperMock { get; set; }
        public IRepository<Entities.Customer> RepositoryMock { get; set; }
        public IRepository<Entities.OrderHistory> OrderRepositoryMock { get; set; }
        public Entities.Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerDTO CustomerDTO { get; set; }
        public CreateEditCustomerDTO CreateEditDTO { get; set; }
        public CreateEditCustomerAddressDTO CreateEditAddressDTO { get; set; }
        public CustomerCreated CreatedEvent { get; set; }
        public CustomerDeleted DeletedEvent { get; set; }
        public CustomerUpdated UpdatedEvent { get; set; }

        public CreateEditCustomerDTO NewCreateEditCustomerDTO(bool withAddress = true)
            => new Faker<CreateEditCustomerDTO>()
            .CustomInstantiator(f => new CreateEditCustomerDTO
            (
                Name: f.Person.FullName,
                Email: f.Person.Email,
                DocumentNumber: f.Random.AlphaNumeric(10),
                PhoneNumber: f.Person.Phone,
                CellNumber: f.Person.Phone,
                Address: withAddress ? NewCreateEditCustomerAddressDTO() : null
            ));

        public CreateEditCustomerAddressDTO NewCreateEditCustomerAddressDTO()
            => new Faker<CreateEditCustomerAddressDTO>()
            .CustomInstantiator(f => new CreateEditCustomerAddressDTO
            (
                Street: f.Address.StreetName(),
                Number: f.Address.BuildingNumber(),
                PostalCode: f.Address.ZipCode(),
                City: f.Address.City()
            ));

        public CustomerDTO NewCustomerDTO()
            => new Faker<CustomerDTO>()
            .CustomInstantiator(f => new CustomerDTO
            (
                Id             : f.Random.Guid(), 
                Name           : f.Person.FullName,
                Email          : f.Person.Email,
                DocumentNumber : f.Random.AlphaNumeric(10),
                PhoneNumber    : f.Person.Phone,
                CellNumber     : f.Person.Phone,
                Address        : null
            ));

        public Entities.Customer NewCustomer(bool withAddress = true)
            => new Faker<Entities.Customer>()
            .CustomInstantiator(f => new Entities.Customer
            {
                Id = f.Random.Guid(),
                Name = f.Person.FullName,
                Email = f.Person.Email,
                DocumentNumber = f.Random.AlphaNumeric(10),
                PhoneNumber = f.Person.Phone,
                CellNumber = f.Person.Phone,
                Address = withAddress ? NewCustomerAddress() : null
            });

        public Entities.CustomerAddress NewCustomerAddress()
            => new Faker<Entities.CustomerAddress>()
            .CustomInstantiator(f => new Entities.CustomerAddress
            {
                Street = f.Address.StreetName(),
                Number = int.Parse(f.Address.BuildingNumber()),
                PostalCode = f.Address.ZipCode(),
                City = f.Address.City(),
                State = f.Address.State(),
                Country = f.Address.Country()
            });

        public Entities.OrderHistory NewOrderHistory(Guid? customerId = null)
            => new Faker<Entities.OrderHistory>()
            .CustomInstantiator(f => new Entities.OrderHistory
            {
                CustomerId = customerId ?? f.Random.Guid(),
                OrderId = f.Random.Guid(),
                Description = f.Random.Words(),
                OrderedDate = f.Date.PastOffset(),
                DeliveryDate = f.Date.FutureOffset()
            });
    }
}