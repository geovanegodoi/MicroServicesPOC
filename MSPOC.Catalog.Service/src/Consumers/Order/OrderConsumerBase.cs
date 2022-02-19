using System.Threading.Tasks;
using MassTransit;
using MSPOC.Events.Catalog;
using MSPOC.Catalog.Service.Entities;
using MSPOC.CrossCutting;
using MSPOC.Events.Order;
using Entity = MSPOC.Catalog.Service.Entities;

namespace MSPOC.Catalog.Service.Consumers
{
    public abstract class OrderConsumerBase
    {
        protected readonly IRepository<Item> _repository;
        protected readonly IPublishEndpoint _publishEndpoint;

        protected OrderConsumerBase(IRepository<Item> repository, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }
        
        protected async Task AddCatalogItemAsync(OrderItemEvent orderItem, Entity.Item catalogItem)
        {
            catalogItem.Quantity += orderItem.Quantity;
            await _repository.UpdateAsync(catalogItem);
        }

        protected async Task SubtractCatalogItemAsync(OrderItemEvent orderItem, Entity.Item catalogItem)
        {
            catalogItem.Quantity -= orderItem.Quantity;
            await _repository.UpdateAsync(catalogItem);
        }

        protected Task NotifyLowQuantityAsync(Entity.Item item)
        {
            if (item.Quantity < 10)
            {
                var message = new CatalogItemLowQuantity(item.Id, item.Quantity);
                _publishEndpoint.Publish<CatalogItemLowQuantity>(message);
            }
            return Task.CompletedTask;
        }
    }
}