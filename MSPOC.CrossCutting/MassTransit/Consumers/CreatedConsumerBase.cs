using System;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;

namespace MSPOC.CrossCutting.MassTransit.Consumers
{
    public abstract class CreatedConsumerBase<TMessage, TEntity, TRepository> : IConsumer<TMessage>
        where TMessage : class
        where TEntity : Entity
        where TRepository : IRepository<TEntity>
    {
        private readonly TRepository _repository;
        private readonly IMapper _mapper;

        public CreatedConsumerBase(TRepository repository, IMapper mapper)
        {            
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var message = context.Message;
            var entity = await _repository.GetAsync(GetMessageId(message));
            if (entity != null) return;
            entity = _mapper.Map<TEntity>(message);
            await _repository.CreateAsync(entity);
        }

        protected abstract Guid GetMessageId(TMessage message);
    }
}