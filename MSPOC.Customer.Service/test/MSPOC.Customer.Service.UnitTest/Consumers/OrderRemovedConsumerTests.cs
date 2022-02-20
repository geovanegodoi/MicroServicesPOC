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
    public class OrderRemovedConsumerTests : OrderEventsConsumerTestsBase<OrderRemoved>
    {
        #pragma warning disable CS4014
        private readonly OrderRemovedConsumer _sut;

        public OrderRemovedConsumerTests()
        {
            _consumeContextMock.Message.Returns(NewOrderRemoved());

            _sut = new OrderRemovedConsumer(_repositoryMock, _mapperMock);
        }

        [Fact]
        public async Task Consome_OrderRemovedWithoutHistory_NotRemoveOrderHistory()
        {
            // Arrange
            OrderHistory historyNotFound = null;
            _repositoryMock.FindAsync(default).ReturnsForAnyArgs(historyNotFound);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).RemoveAsync(default);
        }
        
        [Fact]
        public async Task Consome_OrderRemovedWithHistory_RemoveOrderHistory()
        {
            // Arrange
            OrderHistory historyFound = new();
            _repositoryMock.FindAsync(default).ReturnsForAnyArgs(historyFound);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).RemoveAsync(default);
        }

        private OrderRemoved NewOrderRemoved()
            => new Faker<OrderRemoved>()
            .CustomInstantiator(f => new OrderRemoved
            (
                OrderId    : f.Random.Guid(), 
                CustomerId : f.Random.Guid(),
                OrderItems : new OrderItemEvent[0]
            ));

        #pragma warning restore CS4014
    }
}