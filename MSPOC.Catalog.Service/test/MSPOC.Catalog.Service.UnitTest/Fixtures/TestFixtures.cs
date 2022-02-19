using MSPOC.Events.Order;
using Bogus;
using System;
using MSPOC.Events.Catalog;

namespace MSPOC.Catalog.Service.UnitTest.Fixtures
{
    public class TestFixtures
    {
        public TestFixturesCollections Collections { get; }

        public TestFixtures()
        {
            Collections = new TestFixturesCollections(this);
        }

        public OrderCreated NewOrderCreated(Entities.Item item = null)
            => new Faker<OrderCreated>()
                .CustomInstantiator(f => new OrderCreated
                (
                    OrderId      : f.Random.Guid(),
                    CustomerId   : f.Random.Guid(),
                    Description  : f.Random.Words(),
                    TotalPrice   : f.Random.Decimal(),
                    OrderedDate  : f.Date.PastOffset(),
                    DeliveryDate : f.Date.FutureOffset(),
                    OrderItems   : new OrderItemEvent[] { NewOrderItemEvent(item) } 
                ));

        public OrderUpdated NewOrderUpdated(Entities.Item item = null)
            => new Faker<OrderUpdated>()
                .CustomInstantiator(f => new OrderUpdated
                (
                    OrderId      : f.Random.Guid(),
                    CustomerId   : f.Random.Guid(),
                    Description  : f.Random.Words(),
                    TotalPrice   : f.Random.Decimal(),
                    OrderedDate  : f.Date.PastOffset(),
                    DeliveryDate : f.Date.FutureOffset(),
                    OrderItems   : new OrderItemUpdatedEvent[] { NewOrderItemUpdatedEvent(item, oldQty: 1, newQty: 1) } 
                ));

        public OrderUpdated NewOrderIncreasedUpdated(Entities.Item item = null)
            => new Faker<OrderUpdated>()
                .CustomInstantiator(f => new OrderUpdated
                (
                    OrderId      : f.Random.Guid(),
                    CustomerId   : f.Random.Guid(),
                    Description  : f.Random.Words(),
                    TotalPrice   : f.Random.Decimal(),
                    OrderedDate  : f.Date.PastOffset(),
                    DeliveryDate : f.Date.FutureOffset(),
                    OrderItems   : new OrderItemUpdatedEvent[] { NewOrderItemUpdatedEvent(item, oldQty: 1, newQty: 2) } 
                ));

        public OrderUpdated NewOrderDecreasedUpdated(Entities.Item item = null)
            => new Faker<OrderUpdated>()
                .CustomInstantiator(f => new OrderUpdated
                (
                    OrderId      : f.Random.Guid(),
                    CustomerId   : f.Random.Guid(),
                    Description  : f.Random.Words(),
                    TotalPrice   : f.Random.Decimal(),
                    OrderedDate  : f.Date.PastOffset(),
                    DeliveryDate : f.Date.FutureOffset(),
                    OrderItems   : new OrderItemUpdatedEvent[] { NewOrderItemUpdatedEvent(item, oldQty: 2, newQty: 1) } 
                ));

        public OrderRemoved NewOrderRemoved(Entities.Item item = null)
            => new Faker<OrderRemoved>()
                .CustomInstantiator(f => new OrderRemoved
                (
                    OrderId      : f.Random.Guid(),
                    CustomerId   : f.Random.Guid(),
                    OrderItems   : new OrderItemEvent[] { NewOrderItemEvent(item) } 
                ));

        public OrderItemEvent NewOrderItemEvent(Entities.Item item = null)
            => new Faker<OrderItemEvent>()
                .CustomInstantiator(f => new OrderItemEvent
                (
                    ItemId   : item is null ? f.Random.Guid() : item.Id,
                    Quantity : 1
                ));

        public OrderItemUpdatedEvent NewOrderItemUpdatedEvent(Entities.Item item = null, int oldQty = 0, int newQty = 0)
            => new Faker<OrderItemUpdatedEvent>()
                .CustomInstantiator(f => new OrderItemUpdatedEvent
                (
                    ItemId      : item is null ? f.Random.Guid() : item.Id,
                    OldQuantity : oldQty == 0 ? f.Random.Int() : oldQty,
                    NewQuantity : newQty == 0 ? f.Random.Int() : newQty
                ));
                
        public CatalogItemLowQuantity NewCatalogItemLowQuantity(Entities.Item item)
            => new CatalogItemLowQuantity(item.Id, item.Quantity);

        public Entities.Item NewItem()
            => new Faker<Entities.Item>()
                .RuleFor(i => i.Id          , f => f.Random.Guid())
                .RuleFor(i => i.Name        , f => f.Random.Words())
                .RuleFor(i => i.Description , f => f.Random.Words())
                .RuleFor(i => i.Price       , f => f.Random.Decimal())
                .RuleFor(i => i.Quantity    , f => f.Random.Int(min: 1, max: 1000));
    }
}