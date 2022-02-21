using System;
using AutoMapper;
using MSPOC.CrossCutting;
using MSPOC.CrossCutting.MassTransit.Consumers;
using MSPOC.Events.Customer;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Consumers
{
    public class CustomerUpdatedConsumer : UpdatedConsumerBase<CustomerUpdated, Entity.Customer, IRepository<Entity.Customer>>
    {
        public CustomerUpdatedConsumer(IRepository<Entity.Customer> repository, IMapper mapper) 
            : base(repository, mapper) {}

        protected override Guid GetMessageId(CustomerUpdated message) => message.CustomerId;

        protected override void UpdateMessageToEntity(CustomerUpdated message, Entities.Customer entity)
        {
            entity.Name  = message.Name;
            entity.Email = message.Email;
        }
    }
}