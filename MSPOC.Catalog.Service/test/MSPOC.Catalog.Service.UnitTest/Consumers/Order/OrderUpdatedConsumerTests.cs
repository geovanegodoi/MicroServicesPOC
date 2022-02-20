using System;
using System.Collections.Generic;
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
    public class OrderUpdatedConsumerTests : OrderEventsConsumerTestsBase<OrderUpdated>
    {
        private readonly OrderUpdatedConsumer _sut;

        public OrderUpdatedConsumerTests()
        {
            _sut = new OrderUpdatedConsumer(_repositoryMock, _publisherMock);
        }

        /*
        public async Task Consume(ConsumeContext<OrderUpdated> context)
        {
            var message = context.Message;
            foreach (var item in message.OrderItems)
            {
                var entity = await _repository.GetAsync(item.ItemId);
                if (entity == null) continue;
                await UpdateCatalogItemAsync(item , entity);
                await NotifyLowQuantityAsync(entity);
            }
        }
        private async Task UpdateCatalogItemAsync(OrderItemUpdatedEvent orderItemUpdate, Entity.Item catalogItem)
        {
            var orderItem = MapToOrderItemEvent(orderItemUpdate);

            if (orderItemUpdate.NewQuantity > orderItemUpdate.OldQuantity)
            {
                await SubtractCatalogItemAsync(orderItem, catalogItem);
            }
            else if (orderItemUpdate.NewQuantity < orderItemUpdate.OldQuantity)
            {
                await AddCatalogItemAsync(orderItem, catalogItem);
            }
        }
        protected async Task SubtractCatalogItemAsync(OrderItemEvent orderItem, Entity.Item catalogItem)
        {
            catalogItem.Quantity -= orderItem.Quantity;
            await _repository.UpdateAsync(catalogItem);
        }
        protected Task NotifyLowQuantityAsync(Entity.Item item)
        {
            if (item.Quantity < 10)
            {
                var message = new CatalogItemLowQuantity(item.Id, item.Quantity);
                _publishEndpoint.Publish<CatalogItemLowQuantity>(message);
            }
            return Task.CompletedTask;
        }
        */


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
            => (NewOrderUpdated(orderUpdatedType), NewItem());

        private enum OrderUpdatedType { ItemsIncreased, ItemsDecreased };

        private OrderUpdated NewOrderUpdated(OrderUpdatedType orderUpdatedType)
            => new Faker<OrderUpdated>()
            .CustomInstantiator(f => new OrderUpdated
            (
                OrderId      : f.Random.Guid(), 
                CustomerId   : f.Random.Guid(), 
                Description  : f.Random.Word(), 
                TotalPrice   : f.Random.Decimal(), 
                OrderedDate  : f.Date.PastOffset(), 
                DeliveryDate : f.Date.FutureOffset(), 
                OrderItems   : NewOrderItemUpdateEvents(orderUpdatedType)
            ));

        private IEnumerable<OrderItemUpdatedEvent> NewOrderItemUpdateEvents(OrderUpdatedType orderUpdatedType)
            => new Faker<OrderItemUpdatedEvent>()
            .CustomInstantiator(f => GetOrderItemUpdateByType(orderUpdatedType))
            .Generate(1);

        private OrderItemUpdatedEvent GetOrderItemUpdateByType(OrderUpdatedType orderUpdatedType)
            => orderUpdatedType == OrderUpdatedType.ItemsIncreased ?
            NewOrderItemUpdateIncreasedQuantityEvent() :
            NewOrderItemUpdateDecreasedQuantityEvent();

        private OrderItemUpdatedEvent NewOrderItemUpdateIncreasedQuantityEvent()
            => new Faker<OrderItemUpdatedEvent>()
            .CustomInstantiator(f => new OrderItemUpdatedEvent
            (
                ItemId: f.Random.Guid(),
                OldQuantity : 1,
                NewQuantity: 2
            ));

        private OrderItemUpdatedEvent NewOrderItemUpdateDecreasedQuantityEvent()
            => new Faker<OrderItemUpdatedEvent>()
            .CustomInstantiator(f => new OrderItemUpdatedEvent
            (
                ItemId: f.Random.Guid(),
                OldQuantity : 2,
                NewQuantity: 1
            ));

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