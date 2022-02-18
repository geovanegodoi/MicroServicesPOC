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
    public class CatalogItemDeletedConsumer : DeletedConsumerBase<CatalogItemDeleted, CatalogItem, IRepository<CatalogItem>>
    {
        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository, IMapper mapper) 
             : base(repository, mapper) {}

        protected override Guid GetMessageId(CatalogItemDeleted message) => message.ItemId;
    }
}