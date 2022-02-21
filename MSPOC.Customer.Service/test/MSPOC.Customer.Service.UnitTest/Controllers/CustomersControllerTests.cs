using System;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MSPOC.CrossCutting;
using MSPOC.Customer.Service.Controllers;
using MSPOC.Customer.Service.Models;
using MSPOC.Events.Customer;
using NSubstitute;
using Xunit;

namespace MSPOC.Customer.Service.UnitTest.Controllers
{
    public class CustomersControllerTests
    {
        #pragma warning disable CS4014

        private readonly IMapper _mapperMock;
        private readonly IRepository<Entities.Customer> _repositoryMock;
        private readonly IPublishEndpoint _publishMock;
        private readonly CustomersController _sut;


        public CustomersControllerTests()
        {
            _mapperMock = Substitute.For<IMapper>();
            _repositoryMock = Substitute.For<IRepository<Entities.Customer>>();
            _publishMock = Substitute.For<IPublishEndpoint>();

            _sut = new CustomersController(_mapperMock, _repositoryMock, _publishMock);
        }

        [Fact]
        public async Task GetByIdAsync_CustomerExist_Return200Ok()
        {
            // Arrange
            var customer = NewCustomer();
            _repositoryMock.GetAsync(customer.Id).Returns(customer);

            // Act
            var result = await _sut.GetByIdAsync(customer.Id);

            // Assert
            (result.Result as OkObjectResult).StatusCode.Should().Be(200);
        }
        
        [Fact]
        public async Task GetByIdAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            Entities.Customer customerNotFound = null;
            _repositoryMock.GetAsync(default).Returns(customerNotFound);

            // Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PostAsync_CustomerNotExist_PublishCustomerCreatedEvent()
        {
            // Arrange
            var createDTO       = NewCreateEditCustomerDTO();
            var customer        = NewCustomer();
            var customerCreated = MapToCustomerCreated(customer);
            _mapperMock.Map<Entities.Customer>(createDTO).Returns(customer);
        
            // Act
            await _sut.PostAsync(createDTO);
        
            // Assert
            _publishMock.ReceivedWithAnyArgs(1).Publish<CustomerCreated>(customerCreated);
        }

        [Fact]
        public async Task PutAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            var updateDTO = NewCreateEditCustomerDTO();
            Entities.Customer customerNotFound = null;
            _repositoryMock.GetAsync(default).Returns(customerNotFound);

            // Act
            var result = await _sut.PutAsync(Guid.NewGuid(), updateDTO);

            // Assert
            (result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PutAsync_CustomerExist_PublishCustomerUpdatedEvent()
        {
            // Arrange
            var updateDTO       = NewCreateEditCustomerDTO();
            var customer        = NewCustomer();
            var customerUpdated = MapToCustomerUpdated(customer);
            _repositoryMock.GetAsync(customer.Id).Returns(customer);
        
            // Act
            await _sut.PutAsync(customer.Id, updateDTO);
        
            // Assert
            _publishMock.ReceivedWithAnyArgs(1).Publish<CustomerUpdated>(customerUpdated);
        }

        [Fact]
        public async Task DeleteAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            Entities.Customer customerNotFound = null;
            _repositoryMock.GetAsync(default).Returns(customerNotFound);
        
            // Act
            var result = await _sut.DeleteAsync(Guid.NewGuid());
        
            // Assert
            (result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAsync_CustomerExist_PublishCustomerDeletedEvent()
        {
            // Arrange
            var customer        = NewCustomer();
            var customerDeleted = MapToCustomerDeleted(customer);
            _repositoryMock.GetAsync(customer.Id).Returns(customer);

            // Act
            await _sut.DeleteAsync(customer.Id);
        
            // Assert
            _publishMock.ReceivedWithAnyArgs(1).Publish<CustomerDeleted>(customerDeleted);
        }

        [Fact]
        public async Task GetCustomerAddressAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            Entities.Customer customerNotFound = null;
            _repositoryMock.GetAsync(default).Returns(customerNotFound);
        
            // Act
            var result = await _sut.GetCustomerAddressAsync(Guid.NewGuid());
        
            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }
        
        [Fact]
        public async Task GetCustomerAddressAsync_CustomerAddressNotExist_Return404NotFound()
        {
            // Arrange
            var customerWithoutAddress = NewCustomer(withAddress: false);
            _repositoryMock.GetAsync(default).Returns(customerWithoutAddress);
        
            // Act
            var result = await _sut.GetCustomerAddressAsync(customerWithoutAddress.Id);
        
            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetCustomerAddressAsync_CustomerAndAddressExist_Return200Ok()
        {
            // Arrange
            var customerWithAddress = NewCustomer(withAddress: true);
            _repositoryMock.GetAsync(customerWithAddress.Id).Returns(customerWithAddress);
        
            // Act
            var result = await _sut.GetCustomerAddressAsync(customerWithAddress.Id);
        
            // Assert
            (result.Result as OkObjectResult).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateCustomerAddressAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            var editAddressDTO = NewCreateEditCustomerAddressDTO();
            Entities.Customer customerNotFound = null;
            _repositoryMock.GetAsync(default).Returns(customerNotFound);
        
            // Act
            var result = await _sut.UpdateCustomerAddressAsync(Guid.NewGuid(), editAddressDTO);
        
            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateCustomerAddressAsync_CustomerExist_PublishCustomerUpdatedEvent()
        {
            // Arrange
            var editAddressDTO  = NewCreateEditCustomerAddressDTO();
            var customer        = NewCustomer(withAddress: true);
            var customerUpdated = MapToCustomerUpdated(customer);
            _repositoryMock.GetAsync(customer.Id).Returns(customer);
        
            // Act
            var result = await _sut.UpdateCustomerAddressAsync(customer.Id, editAddressDTO);
        
            // Assert
            _publishMock.ReceivedWithAnyArgs(1).Publish<CustomerUpdated>(customerUpdated);
        }

        private CreateEditCustomerDTO NewCreateEditCustomerDTO(bool withAddress = true)
            => new Faker<CreateEditCustomerDTO>()
            .CustomInstantiator(f => new CreateEditCustomerDTO
            (
                Name           : f.Person.FullName,
                Email          : f.Person.Email,
                DocumentNumber : f.Random.AlphaNumeric(10),
                PhoneNumber    : f.Person.Phone,
                CellNumber     : f.Person.Phone,
                Address        : withAddress ? NewCreateEditCustomerAddressDTO() : null
            ));

        private CreateEditCustomerAddressDTO NewCreateEditCustomerAddressDTO()
            => new Faker<CreateEditCustomerAddressDTO>()
            .CustomInstantiator(f => new CreateEditCustomerAddressDTO
            (
                Street     : f.Address.StreetName(),
                Number     : f.Address.BuildingNumber(),
                PostalCode : f.Address.ZipCode(),
                City       : f.Address.City()
            ));

        private Entities.Customer NewCustomer(bool withAddress = true)
            => new Faker<Entities.Customer>()
            .CustomInstantiator(f => new Entities.Customer
            {
                Id             = f.Random.Guid(),
                Name           = f.Person.FullName,
                Email          = f.Person.Email,
                DocumentNumber = f.Random.AlphaNumeric(10),
                PhoneNumber    = f.Person.Phone,
                CellNumber     = f.Person.Phone,
                Address        = withAddress ? NewCustomerAddress() : null
            });
        
        private Entities.CustomerAddress NewCustomerAddress()
            => new Faker<Entities.CustomerAddress>()
            .CustomInstantiator(f => new Entities.CustomerAddress
            {
                Street     = f.Address.StreetName(),
                Number     = Convert.ToInt32(f.Address.BuildingNumber()),
                PostalCode = f.Address.ZipCode(),
                City       = f.Address.City(),
                State      = f.Address.State(),
                Country    = f.Address.Country()
            });

        private CustomerCreated MapToCustomerCreated(Entities.Customer customer)
            => new CustomerCreated(CustomerId: customer.Id, Name: customer.Name, Email: customer.Email);

        private CustomerUpdated MapToCustomerUpdated(Entities.Customer customer)
            => new CustomerUpdated(CustomerId: customer.Id, Name: customer.Name, Email: customer.Email);

        private CustomerDeleted MapToCustomerDeleted(Entities.Customer customer)
            => new CustomerDeleted(CustomerId: customer.Id);
        
        #pragma warning restore CS4014
    }
}