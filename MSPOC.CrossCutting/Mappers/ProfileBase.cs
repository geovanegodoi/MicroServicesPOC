using System;
using AutoMapper;

namespace MSPOC.CrossCutting.Mappers
{
    public abstract class ProfileBase<TEntity, TDTO, TCreateEditDTO> : Profile
        where TEntity        : Entity
        where TDTO           : class
        where TCreateEditDTO : class
    {
        public ProfileBase()
        {
            CreateMap<TDTO, TEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(GetValidDateTime<TDTO>()))
                .ReverseMap();

            CreateMap<TCreateEditDTO, TEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(GetValidDateTime<TCreateEditDTO>()))
                .ReverseMap();
        }

        private Func<T, TEntity, DateTimeOffset> GetValidDateTime<T>()
            => (src, dest) => 
                dest.CreatedDate == DateTimeOffset.MinValue ? 
                DateTimeOffset.Now : 
                dest.CreatedDate;
    }

    public abstract class ProfileBaseWithEvents<TEntity, TDTO, TCreateEditDTO> : ProfileBase<TEntity, TDTO, TCreateEditDTO>
        where TEntity        : Entity
        where TDTO           : class
        where TCreateEditDTO : class
    {
        public ProfileBaseWithEvents() : base()
        {
            ConfigureEventsMapping();
        }

        protected abstract void ConfigureEventsMapping();
    }
}