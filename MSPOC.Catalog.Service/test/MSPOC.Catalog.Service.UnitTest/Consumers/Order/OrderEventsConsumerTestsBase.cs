using System.Collections.Generic;
using AutoMapper;
using Bogus;
using MassTransit;
using MSPOC.Catalog.Service.Entities;
using MSPOC.CrossCutting;
using MSPOC.Events.Catalog;
using MSPOC.Events.Order;
using NSubstitute;

namespace MSPOC.Catalog.Service.UnitTest.Consumers
{
    public abstract class OrderEventsConsumerTestsBase<TEvent> where TEvent : class
    {
        #pragma warning disable CS4014

        protected const int ITEM_MINIMUM_QUANTITY = 10;
        protected readonly IRepository<Entities.Item> _repositoryMock;
        protected readonly IMapper _mapperMock;
        protected readonly IPublishEndpoint _publisherMock;
        protected readonly ConsumeContext<TEvent> _consumeContextMock;

        public OrderEventsConsumerTestsBase()
        {
            _repositoryMock     = Substitute.For<IRepository<Entities.Item>>();
            _mapperMock         = Substitute.For<IMapper>();
            _publisherMock      = Substitute.For<IPublishEndpoint>();
            _consumeContextMock = Substitute.For<ConsumeContext<TEvent>>();
        }
        
        protected void VerifyPublishedItemLowQuantityEvent(Entities.Item item, int expectedEvents)
        {
            var lowQuantityEvent = new CatalogItemLowQuantity(item.Id, item.Quantity);
            _publisherMock.Received(expectedEvents).Publish<CatalogItemLowQuantity>(lowQuantityEvent);
        }
        
        #pragma warning restore CS4014
    }
}