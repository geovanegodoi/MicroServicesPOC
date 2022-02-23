using Bogus;
using MSPOC.Events.Customer;
using MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.UnitTest.Fixtures
{
    public class CustomerConsumerFixture
    {
        public Customer NewCustomer()
            => new Faker<Customer>()
            .CustomInstantiator(f => new Customer
            {
                Id    = f.Random.Guid(),
                Email = f.Person.Email
            });

        public CustomerCreated NewCustomerCreated()
            => new Faker<CustomerCreated>()
            .CustomInstantiator(f => new CustomerCreated
            (
                CustomerId : f.Random.Guid(), 
                Name       : f.Commerce.ProductName(),
                Email      : f.Person.Email
            ));

        public CustomerDeleted NewCatalogItemDeleted()
            => new Faker<CustomerDeleted>()
            .CustomInstantiator(f => new CustomerDeleted 
            ( 
                CustomerId : f.Random.Guid() 
            ));

        public CustomerUpdated NewCatalogItemUpdated()
            => new Faker<CustomerUpdated>()
            .CustomInstantiator(f => new CustomerUpdated
            (
                CustomerId : f.Random.Guid(), 
                Name       : f.Commerce.ProductName(),
                Email      : f.Person.Email
            ));
    }
}