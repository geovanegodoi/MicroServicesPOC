using MSPOC.Events.Order;
using Bogus;
using System.Collections.Generic;

namespace MSPOC.Catalog.Service.UnitTest.Fixtures
{
    public class TestFixturesCollections
    {
        private readonly TestFixtures _testFixtures;

        public TestFixturesCollections(TestFixtures testFixtures)
        {
            _testFixtures = testFixtures;
        }

        public IEnumerable<OrderCreated> NewOrderCreated(int count=1)
            => new Faker<OrderCreated>()
                .CustomInstantiator(_ => _testFixtures.NewOrderCreated())
                .Generate(count);

        public IEnumerable<OrderItemEvent> NewOrderItemEvent(int count=1)
            => new Faker<OrderItemEvent>()
                .CustomInstantiator(f => _testFixtures.NewOrderItemEvent())
                .Generate(count);
        
        public IEnumerable<Entities.Item> NewItem(int count=1)
            => new Faker<Entities.Item>()
                .CustomInstantiator(f => _testFixtures.NewItem())
                .Generate(count);
    }
}