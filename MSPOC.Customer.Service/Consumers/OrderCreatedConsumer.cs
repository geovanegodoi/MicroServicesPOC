using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.CrossCutting.MassTransit.Consumers;
using MSPOC.Customer.Service.Entities;
using MSPOC.Events.Order;

namespace MSPOC.Customer.Service.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly IRepository<OrderHistory> _repository;
        private readonly IMapper _mapper;

        public OrderCreatedConsumer(IRepository<OrderHistory> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = context.Message;
            var entity = await _repository.FindAsync(WithCustomerIdAndOrderId(message));
            if (entity != null) return;
            entity = _mapper.Map<OrderHistory>(message);
            await _repository.CreateAsync(entity);
        }

        private Expression<Func<OrderHistory, bool>> WithCustomerIdAndOrderId(OrderCreated message)
            => entity => (entity.CustomerId == message.CustomerId) && (entity.OrderId == message.OrderId);
    }
}