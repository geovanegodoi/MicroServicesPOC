using System;
using MSPOC.Catalog.Service.Entities;
using MSPOC.Events.Catalog;
using MSPOC.Catalog.Service.Models;
using MSPOC.CrossCutting.Mappers;

namespace MSPOC.Catalog.Service.Mappers
{
    public class CatalogProfile : ProfileBaseWithEvents<Item, ItemDTO, CreateEditItemDTO> 
    {
        protected override void ConfigureEventsMapping()
        {
            CreateMap<Item, CatalogItemCreated>()
                .ConstructUsing(src => new CatalogItemCreated(src.Id, src.Name, src.Price));
            
            CreateMap<Item, CatalogItemUpdated>()
                .ConstructUsing(src => new CatalogItemUpdated(src.Id, src.Name, src.Price));

            CreateMap<Item, CatalogItemDeleted>()
                .ConstructUsing(src => new CatalogItemDeleted(src.Id));
        }
    }
}