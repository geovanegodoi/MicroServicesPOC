using System.Collections.Generic;
using Bogus;
using MSPOC.Events.Order;

namespace MSPOC.Catalog.Service.UnitTest.Fixtures
{
    public class ConsumerFixture
    {
        public OrderCreated NewOrderCreated()
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

        public OrderRemoved NewOrderRemoved()
            => new Faker<OrderRemoved>()
            .CustomInstantiator(f => new OrderRemoved
            (
                OrderId      : f.Random.Guid(), 
                CustomerId   : f.Random.Guid(), 
                OrderItems   : NewOrderItemEvents()
            ));

        public IEnumerable<OrderItemEvent> NewOrderItemEvents(int count = 1)
            => new Faker<OrderItemEvent>()
            .CustomInstantiator(f => new OrderItemEvent
            (
                ItemId: f.Random.Guid(),
                Quantity: 1
            ))
            .Generate(count);

        public enum OrderUpdatedType { ItemsIncreased, ItemsDecreased };

        public OrderUpdated NewOrderUpdated(OrderUpdatedType orderUpdatedType)
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

        public IEnumerable<OrderItemUpdatedEvent> NewOrderItemUpdateEvents(OrderUpdatedType orderUpdatedType)
            => new Faker<OrderItemUpdatedEvent>()
            .CustomInstantiator(f => NewOrderItemsByType(orderUpdatedType))
            .Generate(1);

        public OrderItemUpdatedEvent NewOrderItemsByType(OrderUpdatedType orderUpdatedType)
            => orderUpdatedType == OrderUpdatedType.ItemsIncreased ?
            NewOrderItemUpdatedEventIncreasedQuantity() :
            NewOrderItemUpdatedEventDecreasedQuantity();

        public OrderItemUpdatedEvent NewOrderItemUpdatedEventIncreasedQuantity()
            => new Faker<OrderItemUpdatedEvent>()
            .CustomInstantiator(f => new OrderItemUpdatedEvent
            (
                ItemId: f.Random.Guid(),
                OldQuantity : 1,
                NewQuantity: 2
            ));

        public OrderItemUpdatedEvent NewOrderItemUpdatedEventDecreasedQuantity()
            => new Faker<OrderItemUpdatedEvent>()
            .CustomInstantiator(f => new OrderItemUpdatedEvent
            (
                ItemId: f.Random.Guid(),
                OldQuantity : 2,
                NewQuantity: 1
            ));

        public Entities.Item NewItem()
            => new Faker<Entities.Item>()
            .CustomInstantiator(f => new Entities.Item
            {
                Id          = f.Random.Guid(),
                Name        = f.Commerce.ProductName(),
                Description = f.Commerce.ProductDescription(),
                Price       = f.Random.Decimal(),
                Quantity    = f.Random.Int(min: 1, max: 100),
                CreatedDate = f.Date.PastOffset()
            });        
    }
}