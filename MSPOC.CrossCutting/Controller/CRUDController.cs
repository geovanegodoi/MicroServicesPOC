using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace MSPOC.CrossCutting
{
    public abstract class CRUDController<TRepository, TEntity, TDTO, TCreateEditDTO> : ControllerBase
        where TRepository    : IRepository<TEntity>
        where TEntity        : Entity
        where TDTO           : class
        where TCreateEditDTO : class
    {
        protected readonly IMapper _baseMapper;
        protected readonly TRepository _baseRepository;

        protected CRUDController(IMapper baseMapper, TRepository baseRepository)
        {
            _baseMapper     = baseMapper;
            _baseRepository = baseRepository;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDTO>>> GetAsync()
        {
            var entities = await _baseRepository.GetAllAsync();
            var items    = _baseMapper.Map<IEnumerable<TDTO>>(source: entities);
            
            return Ok(items);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDTO>> GetByIdAsync([FromRoute]Guid id)
        {
            var entity = await _baseRepository.GetAsync(id);
            
            return (entity is null) ? 
                NotFound() : Ok(_baseMapper.Map<TDTO>(source: entity));
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDTO>> PostAsync([FromBody]TCreateEditDTO dto)
        {
            var newEntity = _baseMapper.Map<TEntity>(source: dto);
            await _baseRepository.CreateAsync(newEntity);
            var createdItem = _baseMapper.Map<TDTO>(source: newEntity);
            
            return CreatedAtAction(nameof(GetByIdAsync), new { id = newEntity.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> PutAsync([FromRoute]Guid id, [FromBody]TCreateEditDTO dto)
        {
            var existingEntity = await _baseRepository.GetAsync(id);
            if (existingEntity is null) return NotFound();
            _baseMapper.Map(source: dto, destination: existingEntity);
            await _baseRepository.UpdateAsync(existingEntity);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync([FromRoute]Guid id)
        {
            var existingEntity = await _baseRepository.GetAsync(id);
            if (existingEntity is null) return NotFound();
            await _baseRepository.RemoveAsync(existingEntity);
            
            return NoContent();
        }
    }
}