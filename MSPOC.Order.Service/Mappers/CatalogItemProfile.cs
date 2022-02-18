using System;
using AutoMapper;
using MSPOC.Events.Catalog;
using MSPOC.Order.Service.Entities;
using MSPOC.Order.Service.Models;

namespace MSPOC.Order.Service.Mappers
{
    public class CatalogItemProfile : Profile
    {
        public CatalogItemProfile()
        {
            CreateMap<CatalogItemCreated, CatalogItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.ItemId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));
        
            CreateMap<CatalogItemUpdated, CatalogItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.ItemId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));
            
            CreateMap<CatalogItemDeleted, CatalogItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.ItemId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.Now));

            CreateMap<CatalogItem, CatalogItemDTO>();
        }
    }
}