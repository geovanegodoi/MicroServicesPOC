using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using MSPOC.Catalog.Service.Consumers;
using MSPOC.Catalog.Service.Entities;
using MSPOC.Catalog.Service.UnitTest.Fixtures;
using MSPOC.Catalog.Service.UnitTest.TestDoubles;
using MSPOC.CrossCutting;
using MSPOC.Events.Catalog;
using MSPOC.Events.Order;
using NSubstitute;
using Xunit;

namespace MSPOC.Catalog.Service.UnitTest.Consumers
{
    public class OrderUpdatedConsumerTests : IClassFixture<TestFixtures>
    {
        private const int ITEM_MINIMUM_QUANTITY = 10;

        private readonly TestFixtures _fixture;
        private readonly OrderUpdatedConsumer _sut;
        private readonly IRepository<Item> _repositoryStub;
        private readonly IPublishEndpoint _publishMock;

        public OrderUpdatedConsumerTests(TestFixtures testFixtures)
        {
            _fixture = testFixtures;
            _repositoryStub = new RepositoryStub<Entities.Item>();
            _publishMock = Substitute.For<IPublishEndpoint>();

            _sut = new OrderUpdatedConsumer(_repositoryStub, _publishMock);
        }

        [Fact]
        public async Task Consume_OrderUpdated_ShouldSubtractItemQuantity()
        {
            //Arrange
            var catalogItem = await NewItemIntoRepositoryAsync();
            var eventStub = NewOrderUpdatedStub(catalogItem, quantityIncreased: true);
            var eventOrderItem = eventStub.GetMessage().OrderItems.FirstOrDefault();
            var expectedItemQuantity = catalogItem.Quantity - Math.Abs(eventOrderItem.NewQuantity - eventOrderItem.OldQuantity);

            //Act
            await _sut.Consume(eventStub.GetContext());

            //Assert
            catalogItem.Quantity.Should().Be(expectedItemQuantity);
        }

        [Fact]
        public async Task Consume_OrderUpdated_ShouldAddItemQuantity()
        {
            //Arrange
            var catalogItem = await NewItemIntoRepositoryAsync();
            var eventStub = NewOrderUpdatedStub(catalogItem, quantityIncreased: false);
            var eventOrderItem = eventStub.GetMessage().OrderItems.FirstOrDefault();
            var expectedItemQuantity = catalogItem.Quantity + Math.Abs(eventOrderItem.NewQuantity - eventOrderItem.OldQuantity);


            //Act
            await _sut.Consume(eventStub.GetContext());

            //Assert
            catalogItem.Quantity.Should().Be(expectedItemQuantity);
        }

        [Fact]
        public async Task Consume_ItemUnderMinimumQuantity_ShouldNotifyLowQuantityItem()
        {
            //Arrange
            var catalogItem  = await NewItemIntoRepositoryAsync();
            catalogItem.Quantity = ITEM_MINIMUM_QUANTITY;
            var eventStub = NewOrderUpdatedStub(catalogItem);

            //Act
            await _sut.Consume(eventStub.GetContext());
        
            //Assert
            VerifyPublishedEvents(catalogItem, expectedEvents: 1);
        }

        [Fact]
        public async Task Consume_ItemAboveMinimumQuantity_ShouldNotNotifyLowQuantityItem()
        {
            //Arrange
            var catalogItem  = await NewItemIntoRepositoryAsync();
            catalogItem.Quantity = ITEM_MINIMUM_QUANTITY + 10;
            var eventStub = NewOrderUpdatedStub(catalogItem);

            //Act
            await _sut.Consume(eventStub.GetContext());
        
            //Assert
            VerifyPublishedEvents(catalogItem, expectedEvents: 0);
        }
        
        private async Task<Entities.Item> NewItemIntoRepositoryAsync()
        {
            var newItem = _fixture.NewItem();
            await _repositoryStub.CreateAsync(newItem);
            return newItem;
        }

        private ConsumeContextStub<OrderUpdated> NewOrderUpdatedStub(Entities.Item item)
        {
            var orderUpdated = _fixture.NewOrderUpdated(item);
            return new ConsumeContextStub<OrderUpdated>(orderUpdated);
        }

        private ConsumeContextStub<OrderUpdated> NewOrderUpdatedStub(Entities.Item item, bool quantityIncreased)
        {
            var orderUpdated = quantityIncreased ?
                _fixture.NewOrderIncreasedUpdated(item) : 
                _fixture.NewOrderDecreasedUpdated(item);

            return new ConsumeContextStub<OrderUpdated>(orderUpdated);
        }

        private void VerifyPublishedEvents(Entities.Item item, int expectedEvents)
        {
            var lowQuantityEvent = _fixture.NewCatalogItemLowQuantity(item);
            _publishMock.Received(0).Publish<CatalogItemLowQuantity>(expectedEvents);
        }
    }
}