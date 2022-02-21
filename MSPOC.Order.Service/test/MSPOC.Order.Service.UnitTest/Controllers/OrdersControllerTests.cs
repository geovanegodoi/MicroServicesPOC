using System;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MSPOC.CrossCutting;
using MSPOC.Order.Service.Controllers;
using MSPOC.Order.Service.Models;
using MSPOC.Events.Customer;
using NSubstitute;
using Xunit;
using System.Collections.Generic;
using MSPOC.Events.Order;

namespace MSPOC.Order.Service.UnitTest.Controllers
{
    public class OrdersControllerTests
    {
        #pragma warning disable CS4014

        private readonly IMapper _mapperMock;
        private readonly IRepository<Entities.Order> _orderRepositoryMock;
        private readonly IRepository<Entities.CatalogItem> _itemRepositoryMock;
        private readonly IRepository<Entities.Customer> _customerRepositoryMock;
        private readonly IPublishEndpoint _publisherMock;
        private readonly OrdersController _sut;


        public OrdersControllerTests()
        {
            _mapperMock             = Substitute.For<IMapper>();
            _orderRepositoryMock    = Substitute.For<IRepository<Entities.Order>>();
            _itemRepositoryMock     = Substitute.For<IRepository<Entities.CatalogItem>>();
            _customerRepositoryMock = Substitute.For<IRepository<Entities.Customer>>();

            _publisherMock = Substitute.For<IPublishEndpoint>();

            _sut = new OrdersController
            (
                _mapperMock, 
                _orderRepositoryMock, 
                _itemRepositoryMock, 
                _customerRepositoryMock, 
                _publisherMock
            );
        }

        [Fact]
        public async Task GetByIdAsync_OrderExist_Return200Ok()
        {
            // Arrange
            var order = NewOrder();
            _orderRepositoryMock.GetAsync(order.Id).Returns(order);

            // Act
            var result = await _sut.GetByIdAsync(order.Id);

            // Assert
            (result.Result as OkObjectResult).StatusCode.Should().Be(200);
        }
        
        [Fact]
        public async Task GetByIdAsync_OrderNotExist_Return404NotFound()
        {
            // Arrange
            Entities.Order orderNotExist = null;
            _orderRepositoryMock.GetAsync(default).Returns(orderNotExist);

            // Act
            var action = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            (action.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PostAsync_CustomerNotExist_Return400BadRequest()
        {
            // Arrange
            var createDTO = NewCreateEditOrderDTO();
            Entities.Customer customerNotExist = null;
            _customerRepositoryMock.GetAsync(default).Returns(customerNotExist);

            // Act
            var action = await _sut.PostAsync(createDTO);

            // Assert
            (action.Result as BadRequestObjectResult).StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PostAsync_ItemNotExist_Return400BadRequest()
        {
            // Arrange
            var createDTO = NewCreateEditOrderDTO();
            Entities.CatalogItem itemNotExist = null;
            _itemRepositoryMock.GetAsync(default).Returns(itemNotExist);

            // Act
            var action = await _sut.PostAsync(createDTO);

            // Assert
            (action.Result as BadRequestObjectResult).StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PostAsync_OrderNotExist_PublishOrderCreatedEvent()
        {
            // Arrange
            SetupExistingCustomerAndItem();
            var createDTO    = NewCreateEditOrderDTO();
            var order        = NewOrder();
            var orderDTO     = MapToOrderDTO(order);
            var orderCreated = MapToOrderCreated(order);
            _mapperMock.Map<Entities.Order>(createDTO).Returns(order);
            _mapperMock.Map<OrderDTO>(order).Returns(orderDTO);

            // Act
            await _sut.PostAsync(createDTO);
        
            // Assert
            _publisherMock.ReceivedWithAnyArgs(1).Publish<OrderCreated>(orderCreated);
        }

        private void SetupExistingCustomerAndItem()
        {
            var customer = NewCustomer();
            _customerRepositoryMock.GetAsync(default).ReturnsForAnyArgs(customer);

            var catalogItem = NewCatalogItem();
            _itemRepositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);
        }

        // [Fact]
        // public async Task PutAsync_CustomerNotExist_Return404NotFound()
        // {
        //     // Arrange
        //     var updateDTO = NewCreateEditCustomerDTO();
        //     Entities.Customer customerNotFound = null;
        //     _repositoryMock.GetAsync(default).Returns(customerNotFound);

        //     // Act
        //     var result = await _sut.PutAsync(Guid.NewGuid(), updateDTO);

        //     // Assert
        //     (result as NotFoundResult).StatusCode.Should().Be(404);
        // }

        // [Fact]
        // public async Task PutAsync_CustomerExist_PublishCustomerUpdatedEvent()
        // {
        //     // Arrange
        //     var updateDTO       = NewCreateEditCustomerDTO();
        //     var customer        = NewCustomer();
        //     var customerUpdated = MapToCustomerUpdated(customer);
        //     _repositoryMock.GetAsync(customer.Id).Returns(customer);
        
        //     // Act
        //     await _sut.PutAsync(customer.Id, updateDTO);
        
        //     // Assert
        //     _publisherMock.ReceivedWithAnyArgs(1).Publish<CustomerUpdated>(customerUpdated);
        // }

        // [Fact]
        // public async Task DeleteAsync_CustomerNotExist_Return404NotFound()
        // {
        //     // Arrange
        //     Entities.Customer customerNotFound = null;
        //     _repositoryMock.GetAsync(default).Returns(customerNotFound);
        
        //     // Act
        //     var result = await _sut.DeleteAsync(Guid.NewGuid());
        
        //     // Assert
        //     (result as NotFoundResult).StatusCode.Should().Be(404);
        // }

        // [Fact]
        // public async Task DeleteAsync_CustomerExist_PublishCustomerDeletedEvent()
        // {
        //     // Arrange
        //     var customer        = NewCustomer();
        //     var customerDeleted = MapToCustomerDeleted(customer);
        //     _repositoryMock.GetAsync(customer.Id).Returns(customer);

        //     // Act
        //     await _sut.DeleteAsync(customer.Id);
        
        //     // Assert
        //     _publisherMock.ReceivedWithAnyArgs(1).Publish<CustomerDeleted>(customerDeleted);
        // }

        // [Fact]
        // public async Task GetCustomerAddressAsync_CustomerNotExist_Return404NotFound()
        // {
        //     // Arrange
        //     Entities.Customer customerNotFound = null;
        //     _repositoryMock.GetAsync(default).Returns(customerNotFound);
        
        //     // Act
        //     var result = await _sut.GetCustomerAddressAsync(Guid.NewGuid());
        
        //     // Assert
        //     (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        // }
        
        // [Fact]
        // public async Task GetCustomerAddressAsync_CustomerAddressNotExist_Return404NotFound()
        // {
        //     // Arrange
        //     var customerWithoutAddress = NewCustomer(withAddress: false);
        //     _repositoryMock.GetAsync(default).Returns(customerWithoutAddress);
        
        //     // Act
        //     var result = await _sut.GetCustomerAddressAsync(customerWithoutAddress.Id);
        
        //     // Assert
        //     (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        // }

        // private CreateEditCustomerDTO NewCreateEditCustomerDTO(bool withAddress = true)
        //     => new Faker<CreateEditCustomerDTO>()
        //     .CustomInstantiator(f => new CreateEditCustomerDTO
        //     (
        //         Name           : f.Person.FullName,
        //         Email          : f.Person.Email,
        //         DocumentNumber : f.Random.AlphaNumeric(10),
        //         PhoneNumber    : f.Person.Phone,
        //         CellNumber     : f.Person.Phone,
        //         Address        : withAddress ? NewCreateEditCustomerAddressDTO() : null
        //     ));

        // private CreateEditCustomerAddressDTO NewCreateEditCustomerAddressDTO()
        //     => new Faker<CreateEditCustomerAddressDTO>()
        //     .CustomInstantiator(f => new CreateEditCustomerAddressDTO
        //     (
        //         Street     : f.Address.StreetName(),
        //         Number     : f.Address.BuildingNumber(),
        //         PostalCode : f.Address.ZipCode(),
        //         City       : f.Address.City()
        //     ));

        private Entities.Order NewOrder()
            => new Faker<Entities.Order>()
            .CustomInstantiator(f => new Entities.Order
            {
                Id           = f.Random.Guid(),
                Number       = f.Random.AlphaNumeric(10),
                Description  = f.Random.Words(10),
                DeliveryDate = f.Date.FutureOffset(),
                OrderItems   = NewOrderItems(),
                Customer     = NewCustomer(),
            });
        
        private IEnumerable<Entities.OrderItem> NewOrderItems(int count = 1)
            => new Faker<Entities.OrderItem>()
            .CustomInstantiator(f => NewOrderItem())
            .Generate(count);

        private Entities.OrderItem NewOrderItem()
            => new Faker<Entities.OrderItem>()
            .CustomInstantiator(f => new Entities.OrderItem
            {
                Id       = f.Random.Guid(),
                Name     = f.Commerce.ProductName(),
                Price    = decimal.Parse(f.Commerce.Price()),
                Quantity = f.Random.Int(min: 1, max: 10)
            });
        
        private Entities.Customer NewCustomer()
            => new Faker<Entities.Customer>()
            .CustomInstantiator(f => new Entities.Customer
            {
                Id    = f.Random.Guid(),
                Name  = f.Person.FullName,
                Email = f.Person.Email
            });

        private Entities.CatalogItem NewCatalogItem()
            => new Faker<Entities.CatalogItem>()
            .CustomInstantiator(f => new Entities.CatalogItem
            {
                Id    = f.Random.Guid(),
                Name  = f.Commerce.ProductName(),
                Price = decimal.Parse(f.Commerce.Price())
            });

        private CreateEditOrderDTO NewCreateEditOrderDTO()
            => new Faker<CreateEditOrderDTO>()
            .CustomInstantiator(f => new CreateEditOrderDTO
            (
                Number       : f.Random.AlphaNumeric(10),
                Description  : f.Random.Words(10),
                DeliveryDate : f.Date.FutureOffset(),
                CustomerId   : f.Random.Guid(),
                OrderItems   : NewCreateEditOrderItemsDTO()
            ));

        private IEnumerable<CreateEditOrderItemDTO> NewCreateEditOrderItemsDTO(int count=1)
            => new Faker<CreateEditOrderItemDTO>()
            .CustomInstantiator(f => NewCreateEditOrderItemDTO())
            .Generate(count);

        private CreateEditOrderItemDTO NewCreateEditOrderItemDTO()
            => new Faker<CreateEditOrderItemDTO>()
            .CustomInstantiator(f => new CreateEditOrderItemDTO
            (
                Id: f.Random.Guid(), 
                Quantity: f.Random.Int(min: 1, max: 10)
            ));

        private OrderDTO MapToOrderDTO(Entities.Order order)
            => new OrderDTO
            (
                Id           : order.Id,
                Number       : order.Number,
                Description  : order.Description,
                DeliveryDate : order.DeliveryDate,
                Customer     : null,
                OrderItems   : null
            );

        private OrderCreated MapToOrderCreated(Entities.Order order)
            => new OrderCreated
            (
                OrderId      : order.Id,
                CustomerId   : order.Customer.Id, 
                Description  : order.Description,
                TotalPrice   : order.CalculateOrderTotalPrice(),
                OrderedDate  : order.CreatedDate,
                DeliveryDate : order.DeliveryDate,
                OrderItems   : new OrderItemEvent[0]
            );

        private OrderUpdated MapToOrderUpdated(Entities.Order order)
            => new OrderUpdated
            (
                OrderId      : order.Id,
                CustomerId   : order.Customer.Id, 
                Description  : order.Description,
                TotalPrice   : order.CalculateOrderTotalPrice(),
                OrderedDate  : order.CreatedDate,
                DeliveryDate : order.DeliveryDate,
                OrderItems   : new OrderItemUpdatedEvent[0]
            );

        private OrderRemoved MapToOrderRemoved(Entities.Order order)
            => new OrderRemoved
            (
                OrderId      : order.Id,
                CustomerId   : order.Customer.Id,
                OrderItems   : new OrderItemEvent[0]
            );
        
        #pragma warning restore CS4014
    }
}