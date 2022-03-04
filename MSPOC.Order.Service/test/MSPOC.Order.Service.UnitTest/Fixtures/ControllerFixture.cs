using System.Collections.Generic;
using Bogus;
using MSPOC.Order.Service.Models;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.UnitTest.Fixtures
{
    public class ControllerFixture
    {
        public Entity.Order NewOrder()
            => new Faker<Entity.Order>()
            .CustomInstantiator(f => new Entity.Order
            {
                Id           = f.Random.Guid(),
                Number       = f.Random.AlphaNumeric(10),
                Description  = f.Random.Words(10),
                DeliveryDate = f.Date.FutureOffset(),
                OrderItems   = NewOrderItems(),
                Customer     = NewCustomer(),
            });
        
        public IEnumerable<Entity.OrderItem> NewOrderItems(int count = 1)
            => new Faker<Entity.OrderItem>()
            .CustomInstantiator(f => NewOrderItem())
            .Generate(count);

        public Entity.OrderItem NewOrderItem()
            => new Faker<Entity.OrderItem>()
            .CustomInstantiator(f => new Entity.OrderItem
            {
                Id       = f.Random.Guid(),
                Name     = f.Commerce.ProductName(),
                Price    = decimal.Parse(f.Commerce.Price()),
                Quantity = f.Random.Int(min: 1, max: 10)
            });
        
        public Entity.Customer NewCustomer()
            => new Faker<Entity.Customer>()
            .CustomInstantiator(f => new Entity.Customer
            {
                Id    = f.Random.Guid(),
                Name  = f.Person.FullName,
                Email = f.Person.Email
            });

        public Entity.CatalogItem NewCatalogItem()
            => new Faker<Entity.CatalogItem>()
            .CustomInstantiator(f => new Entity.CatalogItem
            {
                Id    = f.Random.Guid(),
                Name  = f.Commerce.ProductName(),
                Price = decimal.Parse(f.Commerce.Price())
            });

        public CreateEditOrderDTO NewCreateEditOrderDTO()
            => new Faker<CreateEditOrderDTO>()
            .CustomInstantiator(f => new CreateEditOrderDTO
            (
                Number       : f.Random.AlphaNumeric(10),
                Description  : f.Random.Words(10),
                DeliveryDate : f.Date.FutureOffset(),
                CustomerId   : f.Random.Guid(),
                OrderItems   : NewCreateEditOrderItemsDTO()
            ));

        public IEnumerable<CreateEditOrderItemDTO> NewCreateEditOrderItemsDTO(int count=1)
            => new Faker<CreateEditOrderItemDTO>()
            .CustomInstantiator(f => NewCreateEditOrderItemDTO())
            .Generate(count);

        public CreateEditOrderItemDTO NewCreateEditOrderItemDTO()
            => new Faker<CreateEditOrderItemDTO>()
            .CustomInstantiator(f => new CreateEditOrderItemDTO
            (
                Id: f.Random.Guid(), 
                Quantity: f.Random.Int(min: 1, max: 10)
            ));

    }
}