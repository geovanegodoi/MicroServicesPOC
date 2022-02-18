using System.Threading.Tasks;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.Events.Order;
using Entity = MSPOC.Catalog.Service.Entities;

namespace MSPOC.Catalog.Service.Consumers
{
    public class OrderUpdatedConsumer : OrderConsumerBase, IConsumer<OrderUpdated>
    {
        public OrderUpdatedConsumer(IRepository<Entity.Item> repository, IPublishEndpoint publishEndpoint)
            : base(repository, publishEndpoint) {}

        public async Task Consume(ConsumeContext<OrderUpdated> context)
        {
            var message = context.Message;
            foreach (var item in message.OrderItems)
            {
                var entity = await _repository.GetAsync(item.ItemId);
                if (entity == null) continue;
                await UpdateCatalogItemAsync(item , entity);
                await NotifyLowQuantityAsync(entity);
            }
        }

        private async Task UpdateCatalogItemAsync(OrderItemEvent orderItem, Entity.Item catalogItem)
        {
            if (orderItem.Quantity > catalogItem.Quantity) 
                await AddCatalogItemAsync(orderItem, catalogItem);
            else
                await SubtractCatalogItemAsync(orderItem, catalogItem);
        }

    }
}