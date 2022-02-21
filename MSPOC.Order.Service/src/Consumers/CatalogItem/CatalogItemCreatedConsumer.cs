using System;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using MSPOC.Events.Catalog;
using MSPOC.CrossCutting;
using MSPOC.CrossCutting.MassTransit.Consumers;
using MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Consumers
{
    public class CatalogItemCreatedConsumer : CreatedConsumerBase<CatalogItemCreated, CatalogItem, IRepository<CatalogItem>>
    {
        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository, IMapper mapper) 
             : base(repository, mapper) {}

        protected override Guid GetMessageId(CatalogItemCreated message) => message.ItemId;
    }
}