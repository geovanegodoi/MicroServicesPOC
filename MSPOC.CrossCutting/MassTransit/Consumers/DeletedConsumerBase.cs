using System;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;

namespace MSPOC.CrossCutting.MassTransit.Consumers
{
    public abstract class DeletedConsumerBase<TMessage, TEntity, TRepository> : IConsumer<TMessage>
        where TMessage : class
        where TEntity : Entity
        where TRepository : IRepository<TEntity>
    {
        private readonly TRepository _repository;
        private readonly IMapper _mapper;

        public DeletedConsumerBase(TRepository repository, IMapper mapper)
        {            
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var message = context.Message;
            var entity = await _repository.GetAsync(GetMessageId(message));
            if (entity == null) return;
            entity = _mapper.Map<TEntity>(message);
            await _repository.RemoveAsync(entity);
        }

        protected abstract Guid GetMessageId(TMessage message);
    }
}