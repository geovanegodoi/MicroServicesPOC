using Microsoft.AspNetCore.Mvc;
using MSPOC.Customer.Service.Mappers;
using MSPOC.Customer.Service.Models;
using MSPOC.CrossCutting;
using System;
using System.Threading.Tasks;
using AutoMapper;
using MSPOC.Events.Customer;
using MassTransit;
using Entity = MSPOC.Customer.Service.Entities;

namespace MSPOC.Customer.Service.Controllers
{ 
    [ApiController]
    [Route("customers")]
    public class CustomersController 
        : CRUDControllerWithMessageBroker<IRepository<Entity.Customer>, Entity.Customer, CustomerDTO, CreateEditCustomerDTO, CustomerCreated, CustomerUpdated, CustomerDeleted>
    {
        public CustomersController(IMapper mapper, IRepository<Entity.Customer> customerRepository, IPublishEndpoint publishEndpoint) 
            : base(mapper, baseRepository: customerRepository, baseEndpoint: publishEndpoint)
        {}

        [HttpGet("{customerId}/address")]
        public async Task<ActionResult<CustomerAddressDTO>> GetCustomerAddressAsync([FromRoute]Guid customerId)
        {
            var entity = await _baseRepository.GetAsync(customerId);
            if (entity is null || entity.Address is null) return NotFound();
            var addressDTO = _baseMapper.Map<CustomerAddressDTO>(entity.Address);
            
            return Ok(addressDTO);
        }

        [HttpPut("{customerId}/address")]
        public async Task<ActionResult<CustomerAddressDTO>> UpdateCustomerAddressAsync([FromRoute]Guid customerId, [FromBody]CreateEditCustomerAddressDTO customerAddressDTO)
        {
            var existingEntity = await _baseRepository.GetAsync(customerId);
            if (existingEntity is null) return NotFound();
            if (existingEntity.Address is null) existingEntity.Address = new Entity.CustomerAddress();
            _baseMapper.Map(customerAddressDTO, existingEntity.Address);
            await _baseRepository.UpdateAsync(existingEntity);
            await base.PublishEvent<Entities.Customer, CustomerUpdated>(existingEntity);

            return NoContent();
        }
    }
}