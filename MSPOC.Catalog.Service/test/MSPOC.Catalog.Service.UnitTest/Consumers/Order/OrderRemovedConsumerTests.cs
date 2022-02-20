using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MassTransit;
using MSPOC.Catalog.Service.Consumers;
using MSPOC.Events.Order;
using NSubstitute;
using Xunit;

namespace MSPOC.Catalog.Service.UnitTest.Consumers
{
    public class OrderDeletedConsumerTests : OrderEventsConsumerTestsBase<OrderRemoved>
    {
        private readonly OrderDeletedConsumer _sut;

        public OrderDeletedConsumerTests()
        {
            _sut = new OrderDeletedConsumer(_repositoryMock, _publisherMock);
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
            => (NewOrderRemoved(), NewItem());

        private OrderRemoved NewOrderRemoved()
            => new Faker<OrderRemoved>()
            .CustomInstantiator(f => new OrderRemoved
            (
                OrderId      : f.Random.Guid(), 
                CustomerId   : f.Random.Guid(), 
                OrderItems   : NewOrderItemEvents()
            ));

        private int GetExpectedAddedQuantity(OrderRemoved orderEvent, Entities.Item item)
        {
            var orderItem = orderEvent.OrderItems.FirstOrDefault();
            return (item.Quantity + orderItem.Quantity);
        }
    }
}