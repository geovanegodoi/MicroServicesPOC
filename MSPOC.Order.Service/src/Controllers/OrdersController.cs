using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MSPOC.CrossCutting;
using MSPOC.Events.Order;
using MSPOC.Order.Service.Entities;
using MSPOC.Order.Service.Models;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController 
        : CRUDControllerWithMessageBroker<IRepository<Entity.Order>, 
                                          Entity.Order, 
                                          OrderDTO, 
                                          CreateEditOrderDTO, 
                                          OrderCreated, 
                                          OrderUpdated, 
                                          OrderRemoved>
    {
        private readonly IRepository<CatalogItem> _catalogItemRepository;
        private readonly IRepository<Entity.Customer> _customerRepository;

        public OrdersController(IMapper                         baseMapper, 
                                IRepository<Entity.Order>       ordersRepository, 
                                IRepository<Entity.CatalogItem> catalogItemRepository, 
                                IRepository<Entity.Customer>    customerRepository, 
                                IPublishEndpoint                publishEndpoint) 
            : base(baseMapper, baseRepository: ordersRepository, baseEndpoint: publishEndpoint)
        {
            _catalogItemRepository = catalogItemRepository;
            _customerRepository    = customerRepository;
        }

        [HttpPost]
        public override async Task<ActionResult<OrderDTO>> PostAsync([FromBody] CreateEditOrderDTO orderDTO)
        {
            if (CheckInputParametersUnavailable(orderDTO, out var actionResult))
            {
                return actionResult;
            }
            var newOrder = _baseMapper.Map<Entity.Order>(source: orderDTO);
            await LoadOrdersDetailsFromRepository(orderDTO, newOrder);
            await _baseRepository.CreateAsync(newOrder);
            await base.PublishEvent<Entity.Order, OrderCreated>(newOrder);
            var createdDTO = _baseMapper.Map<OrderDTO>(source: newOrder);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdDTO.Id }, createdDTO);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> PutAsync([FromRoute] Guid id, [FromBody] CreateEditOrderDTO orderDTO)
        {
            if (CheckInputParametersUnavailable(orderDTO, out var actionResult))
            {
                return actionResult;
            }
            var existingOrder = await _baseRepository.GetAsync(id);
            if (existingOrder == null) return NotFound("Order ID not found");
            var existingOrderItems = existingOrder.OrderItems;
            _baseMapper.Map(source: orderDTO, destination: existingOrder);
            await LoadOrdersDetailsFromRepository(orderDTO, existingOrder);
            await _baseRepository.UpdateAsync(existingOrder);
            await PublishUpdateEvent(existingOrder, existingOrderItems);

            return NoContent();
        }

        [HttpGet("items/available")]
        public async Task<ActionResult<IEnumerable<CatalogItemDTO>>> GetAvailableItemsAsync()
        {
            var entities = await _catalogItemRepository.GetAllAsync();
            var items    = _baseMapper.Map<IEnumerable<CatalogItemDTO>>(source: entities);
            
            return Ok(items);
        }

        [HttpGet("customers/available")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAvailableCustomersAsync()
        {
            var entities = await _customerRepository.GetAllAsync();
            var items    = _baseMapper.Map<IEnumerable<CustomerDTO>>(source: entities);
            
            return Ok(items);
        }

        [HttpGet("{orderId}/items")]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetOrderItemsAsync([FromRoute]Guid orderId)
        {
            var entities = await _baseRepository.GetAsync(orderId);
            var items    = _baseMapper.Map<IEnumerable<OrderItemDTO>>(source: entities);

            return items.Any() ? Ok(items) : NotFound();
        }

        private bool CheckInputParametersUnavailable(CreateEditOrderDTO order, out ActionResult actionResult)
        {
            var invalid = true;

            if (CheckCustomerUnavailable(order, out var curtomerErrors))
            {
                actionResult = BadRequest(curtomerErrors);
            }
            else if (CheckItemsUnavailable(order, out var itemsErrors))
            {
                actionResult = BadRequest(itemsErrors);
            }
            else
            {
                actionResult  = null;
                invalid = false;
            }
            return invalid;
        }

        private bool CheckCustomerUnavailable(CreateEditOrderDTO order, out List<string> errors)
        {
            errors = new List<string>();
            var entity = _customerRepository.GetAsync(order.CustomerId).GetAwaiter().GetResult();
            if (entity == null)
            {
                errors.Add($"Customer [{order.CustomerId}] not available");
            }
            return errors.Any();
        }

        private bool CheckItemsUnavailable(CreateEditOrderDTO order, out List<string> errors)
        {
            errors = new List<string>();
            foreach (var item in order.OrderItems)
            {
                var entity = _catalogItemRepository.GetAsync(item.Id).GetAwaiter().GetResult();
                if (entity == null)
                {
                    errors.Add($"Item [{item.Id}] not available");
                }
            }
            return errors.Any();
        }

        private async Task LoadOrdersDetailsFromRepository(CreateEditOrderDTO orderDTO, Entity.Order orderEntity)
        {
            await LoadCustomerDetailsFromRepository(orderDTO.CustomerId, orderEntity);
            await LoadOrderItemsFromRepository(orderEntity);
        }

        private async Task LoadCustomerDetailsFromRepository(Guid customerId, Entity.Order order)
        {
            var customer   = await _customerRepository.GetAsync(customerId);
            order.Customer = customer;
        }

        private async Task LoadOrderItemsFromRepository(Entity.Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var catalogItem = await _catalogItemRepository.GetAsync(item.Id);
                item.Name  = catalogItem.Name;
                item.Price = catalogItem.Price;
            }
        }

                private async Task PublishUpdateEvent(Entity.Order order, IEnumerable<Entity.OrderItem> oldItems)
        {
            var orderUpdate = new OrderUpdated
            (
                OrderId      : order.Id, 
                CustomerId   : order.Customer.Id, 
                Description  : order.Description, 
                TotalPrice   : order.CalculateOrderTotalPrice(), 
                OrderedDate  : order.CreatedDate, 
                DeliveryDate : order.DeliveryDate, 
                OrderItems   : MergeOrderItems(oldItems, newItems: order.OrderItems)
            );
            await _baseEndpoint.Publish<OrderUpdated>(orderUpdate);
        }

        private IEnumerable<OrderItemUpdatedEvent> MergeOrderItems(IEnumerable<OrderItem> oldItems, IEnumerable<OrderItem> newItems)
            =>  from o in oldItems
                join n in newItems 
                on o.Id equals n.Id
                select new OrderItemUpdatedEvent
                (
                    ItemId      : n.Id,
                    OldQuantity : o.Quantity,
                    NewQuantity : n.Quantity
                );
    }
}
