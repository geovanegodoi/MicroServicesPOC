using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MassTransit;
using MSPOC.Catalog.Service.Consumers;
using MSPOC.Catalog.Service.UnitTest.Fixtures;
using MSPOC.Events.Order;
using NSubstitute;
using Xunit;

namespace MSPOC.Catalog.Service.UnitTest.Consumers
{
    public class OrderDeletedConsumerTests : OrderEventsConsumerTestsBase<OrderRemoved>, IClassFixture<ConsumerFixture>
    {
        private readonly OrderDeletedConsumer _sut;
        private readonly ConsumerFixture _fixture;

        public OrderDeletedConsumerTests(ConsumerFixture fixture)
        {
            _sut     = new OrderDeletedConsumer(_repositoryMock, _publisherMock);
            _fixture = fixture;
        }

        [Fact]
        public async Task Consume_OrderRemoved_AddItemQuantity()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem();
            var expectedItemQuantity = GetExpectedAddedQuantity(orderEvent, catalogItem);
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);

            // Assert
            catalogItem.Quantity.Should().Be(expectedItemQuantity);
        }

        [Fact]
        public async Task Consume_ItemUnderMinimumQuantity_ShouldNotifyLowQuantityItem()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem();
            catalogItem.Quantity = ITEM_MINIMUM_QUANTITY - 5;
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            VerifyPublishedItemLowQuantityEvent(catalogItem, expectedEvents: 1);
        }

        [Fact]
        public async Task Consume_ItemAboveMinimumQuantity_ShouldNotNotifyLowQuantityItem()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem();
            catalogItem.Quantity = ITEM_MINIMUM_QUANTITY + 5;
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            VerifyPublishedItemLowQuantityEvent(catalogItem, expectedEvents: 0);
        }
        
        private (OrderRemoved orderEvent, Entities.Item item) NewEventAndItem()
            => (_fixture.NewOrderRemoved(), _fixture.NewItem());
            
        private int GetExpectedAddedQuantity(OrderRemoved orderEvent, Entities.Item item)
        {
            var orderItem = orderEvent.OrderItems.FirstOrDefault();
            return (item.Quantity + orderItem.Quantity);
        }
    }
}