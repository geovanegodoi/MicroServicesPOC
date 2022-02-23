using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bogus;
using MSPOC.Customer.Service.Consumers;
using MSPOC.Customer.Service.Entities;
using MSPOC.Customer.Service.UnitTest.Fixtures;
using MSPOC.Events.Order;
using NSubstitute;
using Xunit;

namespace MSPOC.Customer.Service.UnitTest.Consumers
{

    public class OrderUpdatedConsumerTests : OrderEventsConsumerTestsBase<OrderUpdated>, IClassFixture<ConsumerFixture>
    {
#pragma warning disable CS4014
        private readonly OrderUpdatedConsumer _sut;
        private readonly ConsumerFixture _fixture;

        public OrderUpdatedConsumerTests(ConsumerFixture fixture)
        {
            _sut = new OrderUpdatedConsumer(_repositoryMock, _mapperMock);
            _fixture = fixture;

            _consumeContextMock.Message.Returns(_fixture.NewOrderUpdated());
        }

        [Fact]
        public async Task Consome_OrderUpdatedWithoutHistory_InsertOrderHistory()
        {
            // Arrange
            OrderHistory historyNotFound = null;
            _repositoryMock.FindAsync(default).ReturnsForAnyArgs(historyNotFound);

            // Act
            await _sut.Consume(_consumeContextMock);

            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).CreateAsync(default);
            _repositoryMock.ReceivedWithAnyArgs(0).UpdateAsync(default);
        }

        [Fact]
        public async Task Consome_OrderUpdatedWithHistory_UpdateOrderHistory()
        {
            // Arrange
            OrderHistory historyFound = new();
            _repositoryMock.FindAsync(default).ReturnsForAnyArgs(historyFound);

            // Act
            await _sut.Consume(_consumeContextMock);

            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).UpdateAsync(default);
            _repositoryMock.ReceivedWithAnyArgs(0).CreateAsync(default);
        }

#pragma warning restore CS4014
    }
}