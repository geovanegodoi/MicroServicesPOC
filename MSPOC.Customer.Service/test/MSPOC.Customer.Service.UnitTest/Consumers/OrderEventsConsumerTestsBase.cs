using AutoMapper;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.Customer.Service.Entities;
using NSubstitute;

namespace MSPOC.Customer.Service.UnitTest.Consumers
{
    public abstract class OrderEventsConsumerTestsBase<TEvent> where TEvent : class
    {
        protected readonly IRepository<OrderHistory> _repositoryMock;
        protected readonly IMapper _mapperMock;
        protected readonly ConsumeContext<TEvent> _consumeContextMock;

        public OrderEventsConsumerTestsBase()
        {
            _repositoryMock     = Substitute.For<IRepository<OrderHistory>>();
            _mapperMock         = Substitute.For<IMapper>();
            _consumeContextMock = Substitute.For<ConsumeContext<TEvent>>();
        }
    }
}