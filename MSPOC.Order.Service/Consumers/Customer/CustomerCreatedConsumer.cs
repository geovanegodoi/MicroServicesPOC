using System;
using AutoMapper;
using MSPOC.CrossCutting;
using MSPOC.CrossCutting.MassTransit.Consumers;
using MSPOC.Events.Customer;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Consumers
{
    public class CustomerCreatedConsumer : CreatedConsumerBase<CustomerCreated, Entity.Customer, IRepository<Entity.Customer>>
    {        
        public CustomerCreatedConsumer(IRepository<Entity.Customer> repository, IMapper mapper) 
             : base(repository, mapper) {}

        protected override Guid GetMessageId(CustomerCreated message) => message.CustomerId;
    }
}