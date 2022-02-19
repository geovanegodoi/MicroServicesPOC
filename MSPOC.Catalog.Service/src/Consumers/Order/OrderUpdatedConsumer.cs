using System;
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

        private async Task UpdateCatalogItemAsync(OrderItemUpdatedEvent orderItemUpdate, Entity.Item catalogItem)
        {
            var orderItem = MapToOrderItemEvent(orderItemUpdate);

            if (orderItemUpdate.NewQuantity > orderItemUpdate.OldQuantity)
            {
                await SubtractCatalogItemAsync(orderItem, catalogItem);
            }
            else if (orderItemUpdate.NewQuantity < orderItemUpdate.OldQuantity)
            {
                await AddCatalogItemAsync(orderItem, catalogItem);
            }
        }

        private OrderItemEvent MapToOrderItemEvent(OrderItemUpdatedEvent orderItemUpdated)
            => new OrderItemEvent
            (
                ItemId: orderItemUpdated.ItemId,
                Quantity: Math.Abs(orderItemUpdated.NewQuantity - orderItemUpdated.OldQuantity)
            );
    }
}