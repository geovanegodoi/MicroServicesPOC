using System.Threading.Tasks;
using MassTransit;
using MSPOC.CrossCutting;
using MSPOC.Events.Order;
using Entity = MSPOC.Catalog.Service.Entities;

namespace MSPOC.Catalog.Service.Consumers
{
    public class OrderDeletedConsumer : OrderConsumerBase, IConsumer<OrderRemoved>
    {
        public OrderDeletedConsumer(IRepository<Entity.Item> repository, IPublishEndpoint publishEndpoint)
            : base(repository, publishEndpoint) {}

        public async Task Consume(ConsumeContext<OrderRemoved> context)
        {
            var message = context.Message;
            foreach (var item in message.OrderItems)
            {
                var entity = await _repository.GetAsync(item.ItemId);
                if (entity == null) continue;
                await AddCatalogItemAsync(item , entity);
                await NotifyLowQuantityAsync(entity);
            }
        }
    }
}