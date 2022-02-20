using System;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MSPOC.Catalog.Service.Entities;
using MSPOC.Events.Catalog;
using MSPOC.Catalog.Service.Models;
using MSPOC.CrossCutting;

namespace MSPOC.Catalog.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController 
        : CRUDControllerWithMessageBroker<IRepository<Item>, 
                                          Item, 
                                          ItemDTO, 
                                          CreateEditItemDTO, 
                                          CatalogItemCreated, 
                                          CatalogItemUpdated, 
                                          CatalogItemDeleted>
    {
        public ItemsController(IMapper mapper, IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
            : base(mapper, baseRepository: itemsRepository, baseEndpoint: publishEndpoint)
        {}
    }
}