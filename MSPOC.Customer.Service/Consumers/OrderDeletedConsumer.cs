using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.Customer.Service.Entities;
using MSPOC.Events.Order;

namespace MSPOC.Customer.Service.Consumers
{
    public class OrderRemovedConsumer : IConsumer<OrderRemoved>
    {
        private readonly IRepository<OrderHistory> _repository;
        private readonly IMapper _mapper;

        public OrderRemovedConsumer(IRepository<OrderHistory> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<OrderRemoved> context)
        {
            var message = context.Message;
            var entity = await _repository.FindAsync(WithCustomerIdAndOrderId(message));
            if (entity == null) return;
            entity = _mapper.Map<OrderHistory>(message);
            await _repository.RemoveAsync(entity);
        }

        private Expression<Func<OrderHistory, bool>> WithCustomerIdAndOrderId(OrderRemoved message)
            => entity => (entity.CustomerId == message.CustomerId) && (entity.OrderId == message.OrderId);
    }
}