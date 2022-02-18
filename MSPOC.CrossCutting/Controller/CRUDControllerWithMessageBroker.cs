using System;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace MSPOC.CrossCutting
{
    public abstract class CRUDControllerWithMessageBroker<TRepository, TEntity, TDTO, TCreateEditDTO, TPostEvent, TPutEvent, TDeleteEvent> 
        : CRUDController<TRepository, TEntity, TDTO, TCreateEditDTO>
        where TRepository    : IRepository<TEntity>
        where TEntity        : Entity
        where TDTO           : class
        where TCreateEditDTO : class
        where TPostEvent     : class
        where TPutEvent      : class
        where TDeleteEvent   : class
    {
        protected readonly IPublishEndpoint _baseEndpoint;

        public CRUDControllerWithMessageBroker(IMapper baseMapper, TRepository baseRepository, IPublishEndpoint baseEndpoint)
            : base(baseMapper, baseRepository)
        {
            _baseEndpoint = baseEndpoint;
        }

        [HttpPost]
        public override async Task<ActionResult<TDTO>> PostAsync([FromBody]TCreateEditDTO dto)
        {
            var newEntity = _baseMapper.Map<TEntity>(source: dto);
            await _baseRepository.CreateAsync(newEntity);
            var createdItem = _baseMapper.Map<TDTO>(source: newEntity);
            await PublishEvent<TEntity, TPostEvent>(newEntity);
            
            return CreatedAtAction(nameof(GetByIdAsync), new { id = newEntity.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> PutAsync([FromRoute]Guid id, [FromBody]TCreateEditDTO dto)
        {
            var existingEntity = await _baseRepository.GetAsync(id);
            if (existingEntity is null) return NotFound();
            _baseMapper.Map(source: dto, destination: existingEntity);
            await _baseRepository.UpdateAsync(existingEntity);
            await PublishEvent<TEntity, TPutEvent>(existingEntity);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync([FromRoute]Guid id)
        {
            var existingEntity = await _baseRepository.GetAsync(id);
            if (existingEntity is null) return NotFound();
            await _baseRepository.RemoveAsync(existingEntity);
            await PublishEvent<TEntity, TDeleteEvent>(existingEntity);
            
            return NoContent();
        }

        protected async Task PublishEvent<TInput, TEvent>(TInput input)
            where TInput : class
            where TEvent : class
        {
            var eventToSend = _baseMapper.Map<TEvent>(source: input);
            await _baseEndpoint.Publish(eventToSend);
        }
    }
}