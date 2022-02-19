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
    public class OrderUpdatedConsumer : IConsumer<OrderUpdated>
    {
        private readonly IRepository<OrderHistory> _repository;
        private readonly IMapper _mapper;

        public OrderUpdatedConsumer(IRepository<OrderHistory> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<OrderUpdated> context)
        {
            var message = context.Message;
            var entity = await _repository.FindAsync(WithCustomerIdAndOrderId(message));
            if (entity == null)
            {
                entity = _mapper.Map<OrderHistory>(message);
                await _repository.CreateAsync(entity);
            }
            else
            {
                UpdateMessageToEntity(message, entity);
                await _repository.UpdateAsync(entity);
            }
        }

        private Expression<Func<OrderHistory, bool>> WithCustomerIdAndOrderId(OrderUpdated message)
            => entity => (entity.CustomerId == message.CustomerId) && (entity.OrderId == message.OrderId);

        private void UpdateMessageToEntity(OrderUpdated message, OrderHistory entity)
        {
            entity.Description  = message.Description;
            entity.TotalPrice   = message.TotalPrice;
            entity.DeliveryDate = message.DeliveryDate;
        }
    }
}