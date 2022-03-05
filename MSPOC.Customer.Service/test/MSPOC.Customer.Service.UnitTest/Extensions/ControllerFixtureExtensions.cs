using System;
using MSPOC.Customer.Service.Models;
using MSPOC.Customer.Service.UnitTest.Fixtures;
using MSPOC.Events.Customer;
using NSubstitute;

namespace MSPOC.Customer.Service.UnitTest.Extensions
{
    public static class ControllerFixtureExtensions
    {
        public static void SetupCustomerExist(this ControllerFixture fixture)
        {
            fixture.Customer   = fixture.NewCustomer(withAddress: true);
            fixture.CustomerId = fixture.Customer.Id;
            fixture.RepositoryMock.GetAsync(fixture.Customer.Id).Returns(fixture.Customer);
            fixture.MapperMock.Map<Entities.Customer>(fixture.CreateEditDTO).Returns(fixture.Customer);
            fixture.SetupDTOs(fixture.Customer);
            fixture.SetupEvents(fixture.Customer);
        }

        public static void SetupCustomerNotExist(this ControllerFixture fixture)
        {
            fixture.Customer   = null;
            fixture.CustomerId = Guid.NewGuid();
            fixture.RepositoryMock.GetAsync(default).Returns(fixture.Customer);
            var newCustomer = fixture.NewCustomer();
            fixture.SetupDTOs(newCustomer);
            fixture.SetupEvents(newCustomer);
        }

        public static void SetupCustomerAddressNotExist(this ControllerFixture fixture)
        {
            fixture.Customer   = fixture.NewCustomer(withAddress: false);
            fixture.CustomerId = fixture.Customer.Id;
            fixture.RepositoryMock.GetAsync(fixture.Customer.Id).Returns(fixture.Customer);
            var newCustomer = fixture.NewCustomer();
            fixture.SetupDTOs(newCustomer);
        }

        public static void SetupCustomerOrderNotExist(this ControllerFixture fixture)
        {
            fixture.Customer   = fixture.NewCustomer();
            fixture.CustomerId = fixture.Customer.Id;
        }

        public static void SetupCustomerOrderExist(this ControllerFixture fixture)
        {
            fixture.Customer   = fixture.NewCustomer();
            fixture.CustomerId = fixture.Customer.Id;
            var orderHistory   = fixture.NewOrderHistory(fixture.CustomerId);
            fixture.OrderRepositoryMock.FindAsync(default).Returns(orderHistory);
        }

        private static void SetupDTOs(this ControllerFixture fixture, Entities.Customer customer)
        {
            fixture.CreateEditDTO        = fixture.NewCreateEditCustomerDTO();
            fixture.CreateEditAddressDTO = fixture.NewCreateEditCustomerAddressDTO();
            fixture.CustomerDTO          = fixture.NewCustomerDTO();
            fixture.SetupDTOMappers(customer);
        }

        private static void SetupDTOMappers(this ControllerFixture fixture, Entities.Customer customer)
        {
            fixture.MapperMock
                   .Map<Entities.Customer>(fixture.CreateEditDTO)
                   .Returns(customer);

            fixture.MapperMock
                   .Map<CustomerDTO>(customer)
                   .Returns(fixture.CustomerDTO);
        }

        private static void SetupEvents(this ControllerFixture fixture, Entities.Customer customer)
        {
            fixture.CreatedEvent = customer.AsCustomerCreated();
            fixture.DeletedEvent = customer.AsCustomerDeleted();
            fixture.UpdatedEvent = customer.AsCustomerUpdated();
            fixture.SetupEventsMappers(customer);
        }

        private static void SetupEventsMappers(this ControllerFixture fixture, Entities.Customer customer)
        {
            fixture.MapperMock
                   .Map<CustomerCreated>(customer)
                   .Returns(fixture.CreatedEvent);

            fixture.MapperMock
                   .Map<CustomerDeleted>(customer)
                   .Returns(fixture.DeletedEvent);

            fixture.MapperMock
                   .Map<CustomerUpdated>(customer)
                   .Returns(fixture.UpdatedEvent);
        }

        private static CustomerCreated AsCustomerCreated(this Entities.Customer customer)
            => new CustomerCreated(CustomerId: customer.Id, Name: customer.Name, Email: customer.Email);

        private static CustomerUpdated AsCustomerUpdated(this Entities.Customer customer)
            => new CustomerUpdated(CustomerId: customer.Id, Name: customer.Name, Email: customer.Email);

        private static CustomerDeleted AsCustomerDeleted(this Entities.Customer customer)
            => new CustomerDeleted(CustomerId: customer.Id);
    }
}