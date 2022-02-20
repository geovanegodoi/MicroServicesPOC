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

    public class OrderCreatedConsumerTests : OrderEventsConsumerTestsBase<OrderCreated>
    {
        private readonly OrderCreatedConsumer _sut;

        public OrderCreatedConsumerTests()
        {
            _sut = new OrderCreatedConsumer(_repositoryMock, _publisherMock);
        }

        [Fact]
        public async Task Consume_OrderCreated_SubtractItemQuantity()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem();
            var expectedItemQuantity = GetExpectedSubtractedQuantity(orderEvent, catalogItem);
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);

            // Assert
            catalogItem.Quantity.Should().Be(expectedItemQuantity);
        }

        [Fact]
        public async Task Consume_ItemUnderMinimumQuantity_NotifyLowQuantityItem()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem();
            catalogItem.Quantity = ITEM_MINIMUM_QUANTITY;
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            VerifyPublishedItemLowQuantityEvent(catalogItem, expectedEvents: 1);
        }

        [Fact]
        public async Task Consume_ItemAboveMinimumQuantity_NotNotifyLowQuantityItem()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem();
            catalogItem.Quantity = ITEM_MINIMUM_QUANTITY + 10;
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            VerifyPublishedItemLowQuantityEvent(catalogItem, expectedEvents: 0);
        }

        private (OrderCreated orderEvent, Entities.Item item) NewEventAndItem()
            => (NewOrderCreated(), NewItem());

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
                OrderItems   : NewOrderItemEvents()
            ));
        private int GetExpectedSubtractedQuantity(OrderCreated orderEvent, Entities.Item item)
        {
            var orderItem = orderEvent.OrderItems.FirstOrDefault();
            return (item.Quantity - orderItem.Quantity);
        }
    }
}