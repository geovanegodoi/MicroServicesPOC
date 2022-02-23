using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.Customer.Service.Consumers;
using MSPOC.Customer.Service.Entities;
using MSPOC.Customer.Service.UnitTest.Fixtures;
using MSPOC.Events.Order;
using NSubstitute;
using Xunit;

namespace MSPOC.Customer.Service.UnitTest.Consumers
{
    public class OrderCreatedConsumerTests : OrderEventsConsumerTestsBase<OrderCreated>, IClassFixture<ConsumerFixture>
    {
        #pragma warning disable CS4014
        private readonly OrderCreatedConsumer _sut;
        private readonly ConsumerFixture _fixture;

        public OrderCreatedConsumerTests(ConsumerFixture fixture)
        {
            _sut = new OrderCreatedConsumer(_repositoryMock, _mapperMock);
            _fixture = fixture;

            _consumeContextMock.Message.Returns(_fixture.NewOrderCreated());
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

        #pragma warning restore CS4014
    }
}