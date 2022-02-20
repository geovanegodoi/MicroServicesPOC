using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.Customer.Service.Consumers;
using MSPOC.Customer.Service.Entities;
using MSPOC.Events.Order;
using NSubstitute;
using Xunit;

namespace MSPOC.Customer.Service.UnitTest.Consumers
{
    public class OrderCreatedConsumerTests : OrderEventsConsumerTestsBase<OrderCreated>
    {
        #pragma warning disable CS4014
        private readonly OrderCreatedConsumer _sut;

        public OrderCreatedConsumerTests()
        {
            _consumeContextMock.Message.Returns(NewOrderCreated());

            _sut = new OrderCreatedConsumer(_repositoryMock, _mapperMock);
        }

        [Fact]
        public async Task Consome_OrderCreatedWithoutHistory_InsertOrderHistory()
        {
            // Arrange
            OrderHistory historyNotFound = null;
            _repositoryMock.FindAsync(default).ReturnsForAnyArgs(historyNotFound);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).CreateAsync(default);
        }
        
        [Fact]
        public async Task Consome_OrderCreatedWithHistory_NotInsertOrderHistory()
        {
            // Arrange
            OrderHistory historyFound = new();
            _repositoryMock.FindAsync(default).ReturnsForAnyArgs(historyFound);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).CreateAsync(default);
        }

        private OrderCreated NewOrderCreated()
            => new Faker<OrderCreated>()
            .CustomInstantiator(f => new OrderCreated
            (
                OrderId      : f.Random.Guid(), 
                CustomerId   : f.Random.Guid(), 
                Description  : f.Random.Word(), 
                TotalPrice   : f.Random.Decimal(), 
                OrderedDate  : f.Date.PastOffset(), 
                DeliveryDate : f.Date.FutureOffset(), 
                OrderItems   : new OrderItemEvent[0]
            ));

        #pragma warning restore CS4014
    }
}