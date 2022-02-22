using System;
using System.Collections.Generic;
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
using static MSPOC.Catalog.Service.UnitTest.Fixtures.ConsumerFixture;

namespace MSPOC.Catalog.Service.UnitTest.Consumers
{
    public class OrderUpdatedConsumerTests : OrderEventsConsumerTestsBase<OrderUpdated>, IClassFixture<ConsumerFixture>
    {
        private readonly OrderUpdatedConsumer _sut;
        private readonly ConsumerFixture _fixture;

        public OrderUpdatedConsumerTests(ConsumerFixture fixture)
        {
            _sut = new OrderUpdatedConsumer(_repositoryMock, _publisherMock);
            _fixture = fixture;
        }

        [Fact]
        public async Task Consume_OrderUpdated_SubtractItemQuantity()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem(OrderUpdatedType.ItemsIncreased);
            var expectedItemQuantity = GetExpectedSubtractedItemQuantity(orderEvent, catalogItem);
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);

            // Assert
            catalogItem.Quantity.Should().Be(expectedItemQuantity);
        }

        [Fact]
        public async Task Consume_OrderUpdated_AddItemQuantity()
        {
            // Arrange
            var (orderEvent, catalogItem) = NewEventAndItem(OrderUpdatedType.ItemsDecreased);
            var expectedItemQuantity = GetExpectedAddedItemQuantity(orderEvent, catalogItem);
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
            var (orderEvent, catalogItem) = NewEventAndItem(OrderUpdatedType.ItemsIncreased);
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
            var (orderEvent, catalogItem) = NewEventAndItem(OrderUpdatedType.ItemsDecreased);
            catalogItem.Quantity = ITEM_MINIMUM_QUANTITY + 10;
            _consumeContextMock.Message.Returns(orderEvent);
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(catalogItem);

            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            VerifyPublishedItemLowQuantityEvent(catalogItem, expectedEvents: 0);
        }

        private (OrderUpdated orderEvent, Entities.Item item) NewEventAndItem(OrderUpdatedType orderUpdatedType)
            => (_fixture.NewOrderUpdated(orderUpdatedType), _fixture.NewItem());

        private int GetExpectedSubtractedItemQuantity(OrderUpdated orderUpdated, Entities.Item item)
        {
            var orderEventItem = orderUpdated.OrderItems.FirstOrDefault();
            var orderUpdatedQuantity = Math.Abs(orderEventItem.NewQuantity - orderEventItem.OldQuantity);
            return (item.Quantity - orderUpdatedQuantity);
        }

        private int GetExpectedAddedItemQuantity(OrderUpdated orderUpdated, Entities.Item item)
        {
            var orderEventItem = orderUpdated.OrderItems.FirstOrDefault();
            var orderUpdatedQuantity = Math.Abs(orderEventItem.NewQuantity - orderEventItem.OldQuantity);
            return (item.Quantity + orderUpdatedQuantity);
        }
    }
}