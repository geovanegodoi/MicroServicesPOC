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
using MSPOC.Order.Service.UnitTest.Fixtures;
using Entity = MSPOC.Order.Service.Entities;
using System.Linq;

namespace MSPOC.Order.Service.UnitTest.Controllers
{
    public class OrdersControllerTests : IClassFixture<ControllerFixture>
    {
        #pragma warning disable CS4014

        private readonly IMapper _mapperMock;
        private readonly IRepository<Entity.Order> _orderRepositoryMock;
        private readonly IRepository<Entity.CatalogItem> _itemRepositoryMock;
        private readonly IRepository<Entity.Customer> _customerRepositoryMock;
        private readonly IPublishEndpoint _publisherMock;
        private readonly OrdersController _sut;
        private readonly ControllerFixture _fixture;

        public OrdersControllerTests(ControllerFixture fixture)
        {
            _mapperMock = Substitute.For<IMapper>();
            _orderRepositoryMock = Substitute.For<IRepository<Entity.Order>>();
            _itemRepositoryMock = Substitute.For<IRepository<Entity.CatalogItem>>();
            _customerRepositoryMock = Substitute.For<IRepository<Entity.Customer>>();

            _publisherMock = Substitute.For<IPublishEndpoint>();

            _sut = new OrdersController
            (
                _mapperMock,
                _orderRepositoryMock,
                _itemRepositoryMock,
                _customerRepositoryMock,
                _publisherMock
            );
            _fixture = fixture;
        }

        [Fact]
        public async Task GetByIdAsync_OrderExist_Return200Ok()
        {
            // Arrange
            var order = _fixture.NewOrder();
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
            Entity.Order orderNotExist = null;
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
            var createDTO = _fixture.NewCreateEditOrderDTO();
            Entity.Customer customerNotExist = null;
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
            var createDTO = _fixture.NewCreateEditOrderDTO();
            Entity.CatalogItem itemNotExist = null;
            _itemRepositoryMock.GetAsync(default).Returns(itemNotExist);

            // Act
            var action = await _sut.PostAsync(createDTO);

            // Assert
            (action.Result as BadRequestObjectResult).StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PostAsync_OrderNotExist_InsertOrderEntity()
        {
            // Arrange
            SetupExistingCustomerAndCatalogItem();
            var createDTO    = _fixture.NewCreateEditOrderDTO();
            var order        = SetupOrderNotExist(createDTO);
            var orderCreated = MapToOrderCreated(order);
            _mapperMock.Map<OrderCreated>(order).Returns(orderCreated);

            // Act
            await _sut.PostAsync(createDTO);
        
            // Assert
            _orderRepositoryMock.Received(1).CreateAsync(order);
        }

        [Fact]
        public async Task PostAsync_OrderNotExist_PublishOrderCreatedEvent()
        {
            // Arrange
            SetupExistingCustomerAndCatalogItem();
            var createDTO    = _fixture.NewCreateEditOrderDTO();
            var order        = SetupOrderNotExist(createDTO);
            var orderCreated = MapToOrderCreated(order);
            _mapperMock.Map<OrderCreated>(order).Returns(orderCreated);

            // Act
            await _sut.PostAsync(createDTO);
        
            // Assert
            _publisherMock.Received(1).Publish<OrderCreated>(orderCreated);
        }

        [Fact]
        public async Task PutAsync_CustomerNotExist_Return400BadRequest()
        {
            // Arrange
            var editDTO = _fixture.NewCreateEditOrderDTO();
            Entity.Customer customerNotExist = null;
            _customerRepositoryMock.GetAsync(default).Returns(customerNotExist);

            // Act
            var action = await _sut.PutAsync(Guid.NewGuid(), editDTO);

            // Assert
            (action as BadRequestObjectResult).StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PutAsync_ItemNotExist_Return400BadRequest()
        {
            // Arrange
            var editDTO = _fixture.NewCreateEditOrderDTO();
            Entity.CatalogItem itemNotExist = null;
            _itemRepositoryMock.GetAsync(default).Returns(itemNotExist);

            // Act
            var action = await _sut.PutAsync(Guid.NewGuid(), editDTO);

            // Assert
            (action as BadRequestObjectResult).StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PutAsync_OrderNotExist_Return404NotFound()
        {
            // Arrange
            SetupExistingCustomerAndCatalogItem();
            var editDTO = _fixture.NewCreateEditOrderDTO();
            Entity.Order orderNotExist = null;
            _orderRepositoryMock.GetAsync(default).Returns(orderNotExist);

            // Act
            var action = await _sut.PutAsync(Guid.NewGuid(), editDTO);
        
            // Assert
            (action as NotFoundObjectResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PutAsync_OrderExist_UpdateOrderEntity()
        {
            // Arrange
            SetupExistingCustomerAndCatalogItem();
            var editDTO      = _fixture.NewCreateEditOrderDTO();
            var order        = SetupOrderExist(editDTO);
            var orderUpdated = MapToOrderUpdated(order);
            _mapperMock.Map<OrderUpdated>(order).Returns(orderUpdated);

            // Act
            await _sut.PutAsync(order.Id, editDTO);
        
            // Assert
            _orderRepositoryMock.Received(1).UpdateAsync(order);
        }

        [Fact]
        public async Task PutAsync_OrderExist_PublishOrderUpdatedEvent()
        {
            // Arrange
            SetupExistingCustomerAndCatalogItem();
            var editDTO      = _fixture.NewCreateEditOrderDTO();
            var order        = SetupOrderExist(editDTO);
            var orderUpdated = MapToOrderUpdated(order);
            _mapperMock.Map<OrderUpdated>(order).Returns(orderUpdated);

            // Act
            await _sut.PutAsync(order.Id, editDTO);
        
            // Assert
            _publisherMock.ReceivedWithAnyArgs(1).Publish<OrderUpdated>(orderUpdated);
        }

        [Fact]
        public async Task DeleteAsync_OrderNotExist_Return404NotFound()
        {
            // Arrange
            Entity.Order orderNotExist = null;
            _orderRepositoryMock.GetAsync(default).Returns(orderNotExist);
        
            // Act
            var result = await _sut.DeleteAsync(Guid.NewGuid());
        
            // Assert
            (result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAsync_OrderExist_PublishOrderRemovedEvent()
        {
            // Arrange
            var order        = _fixture.NewOrder();
            var orderRemoved = MapToOrderRemoved(order);
            _orderRepositoryMock.GetAsync(order.Id).Returns(order);
            _mapperMock.Map<OrderRemoved>(order).Returns(orderRemoved);

            // Act
            await _sut.DeleteAsync(order.Id);
        
            // Assert
            _publisherMock.Received(1).Publish<OrderRemoved>(orderRemoved);
        }

        private void SetupExistingCustomerAndCatalogItem()
        {
            var customer = _fixture.NewCustomer();
            _customerRepositoryMock.GetAsync(default).ReturnsForAnyArgs(customer);

            var catalogItem = _fixture.NewCatalogItem();
            _itemRepositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);
        }

        private Entity.Order SetupOrderNotExist(CreateEditOrderDTO createEditDTO)
        {
            var order    = _fixture.NewOrder();
            var orderDTO = MapToOrderDTO(order);
            _mapperMock.Map<Entity.Order>(createEditDTO).Returns(order);
            _mapperMock.Map<OrderDTO>(order).Returns(orderDTO);

            return order;
        }

        private Entity.Order SetupOrderExist(CreateEditOrderDTO createEditDTO)
        {
            var order = _fixture.NewOrder();
            _mapperMock.Map<Entity.Order>(createEditDTO).Returns(order);
            _orderRepositoryMock.GetAsync(order.Id).Returns(order);

            return order;
        }

        private OrderDTO MapToOrderDTO(Entity.Order order)
            => new OrderDTO
            (
                Id           : order.Id,
                Number       : order.Number,
                Description  : order.Description,
                DeliveryDate : order.DeliveryDate,
                Customer     : null,
                OrderItems   : null
            );

        private OrderCreated MapToOrderCreated(Entity.Order order)
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

        private OrderUpdated MapToOrderUpdated(Entity.Order order)
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

        private OrderRemoved MapToOrderRemoved(Entity.Order order)
            => new OrderRemoved
            (
                OrderId      : order.Id,
                CustomerId   : order.Customer.Id,
                OrderItems   : new OrderItemEvent[0]
            );
        
        #pragma warning restore CS4014
    }
}