using System;
using AutoMapper;
using MSPOC.CrossCutting;
using MSPOC.CrossCutting.MassTransit.Consumers;
using MSPOC.Events.Customer;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Consumers
{
    public class CustomerDeletedConsumer : DeletedConsumerBase<CustomerDeleted, Entity.Customer, IRepository<Entity.Customer>>
    {        
        public CustomerDeletedConsumer(IRepository<Entity.Customer> repository, IMapper mapper) 
            : base(repository, mapper) {}

        protected override Guid GetMessageId(CustomerDeleted message) => message.CustomerId;
    }
}