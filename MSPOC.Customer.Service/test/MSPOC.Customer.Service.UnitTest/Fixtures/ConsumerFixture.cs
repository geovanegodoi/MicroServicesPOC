using Bogus;
using MSPOC.Events.Order;

namespace MSPOC.Customer.Service.UnitTest.Fixtures
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
                OrderItems   : new OrderItemEvent[0]
            ));

        public OrderRemoved NewOrderRemoved()
            => new Faker<OrderRemoved>()
            .CustomInstantiator(f => new OrderRemoved
            (
                OrderId    : f.Random.Guid(), 
                CustomerId : f.Random.Guid(),
                OrderItems : new OrderItemEvent[0]
            ));

        public OrderUpdated NewOrderUpdated()
            => new Faker<OrderUpdated>()
            .CustomInstantiator(f => new OrderUpdated
            (
                OrderId      : f.Random.Guid(),
                CustomerId   : f.Random.Guid(),
                Description  : f.Random.Word(),
                TotalPrice   : f.Random.Decimal(),
                OrderedDate  : f.Date.PastOffset(),
                DeliveryDate : f.Date.FutureOffset(),
                OrderItems   : new OrderItemUpdatedEvent[0]
            ));
    }
}