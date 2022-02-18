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
    public class CatalogItemUpdatedConsumer : UpdatedConsumerBase<CatalogItemUpdated, CatalogItem, IRepository<CatalogItem>>
    {
        public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository, IMapper mapper) 
             : base(repository, mapper) {}

        protected override Guid GetMessageId(CatalogItemUpdated message) => message.ItemId;

        protected override void UpdateMessageToEntity(CatalogItemUpdated message, CatalogItem entity)
        {
            entity.Name  = message.Name;
            entity.Price = message.Price;
        }
    }
}