using System;
using MSPOC.Customer.Service.Models;
using MSPOC.CrossCutting.Mappers;
using Entity = MSPOC.Customer.Service.Entities;
using MSPOC.Events.Customer;

namespace MSPOC.Customer.Service.Mappers
{
    public class CustomerProfile : ProfileBaseWithEvents<Entity.Customer, CustomerDTO, CreateEditCustomerDTO> 
    {
        public CustomerProfile() : base()
        {
            ConfigureCustomerAddressMapping();
        }

        protected override void ConfigureEventsMapping()
        {
            CreateMap<Entity.Customer, CustomerCreated>()
                .ConstructUsing(src => new CustomerCreated(src.Id, src.Name, src.Email));

            CreateMap<Entity.Customer, CustomerUpdated>()
                .ConstructUsing(src => new CustomerUpdated(src.Id, src.Name, src.Email));

            CreateMap<Entity.Customer, CustomerDeleted>()
                .ConstructUsing(src => new CustomerDeleted(src.Id));
        }

        private void ConfigureCustomerAddressMapping()
        {
            CreateMap<Entity.CustomerAddress, CustomerAddressDTO>()
                .ReverseMap();

            CreateMap<Entity.CustomerAddress, CreateEditCustomerAddressDTO>()
                .ReverseMap();
        }
    }
}