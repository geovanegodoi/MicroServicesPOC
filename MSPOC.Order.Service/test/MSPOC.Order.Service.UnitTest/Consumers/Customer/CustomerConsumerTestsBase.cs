using AutoMapper;
using Bogus;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.Order.Service.Entities;
using NSubstitute;

namespace MSPOC.Order.Service.UnitTest.Consumers
{
    public abstract class CustomerConsumerTestsBase<TEvent> where TEvent : class
    {
        protected readonly IRepository<Customer> _repositoryMock;
        protected readonly IMapper _mapperMock;
        protected readonly ConsumeContext<TEvent> _consumeContextMock;

        public CustomerConsumerTestsBase()
        {
            _repositoryMock     = Substitute.For<IRepository<Customer>>();
            _mapperMock         = Substitute.For<IMapper>();
            _consumeContextMock = Substitute.For<ConsumeContext<TEvent>>();
        }
    }
}