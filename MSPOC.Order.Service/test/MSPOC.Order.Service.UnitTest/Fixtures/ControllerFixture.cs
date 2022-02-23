using System.Collections.Generic;
using Bogus;
using MSPOC.Order.Service.Models;

namespace MSPOC.Order.Service.UnitTest.Fixtures
{
    public class ControllerFixture
    {
        public Entities.Order NewOrder()
            => new Faker<Entities.Order>()
            .CustomInstantiator(f => new Entities.Order
            {
                Id           = f.Random.Guid(),
                Number       = f.Random.AlphaNumeric(10),
                Description  = f.Random.Words(10),
                DeliveryDate = f.Date.FutureOffset(),
                OrderItems   = NewOrderItems(),
                Customer     = NewCustomer(),
            });
        
        public IEnumerable<Entities.OrderItem> NewOrderItems(int count = 1)
            => new Faker<Entities.OrderItem>()
            .CustomInstantiator(f => NewOrderItem())
            .Generate(count);

        public Entities.OrderItem NewOrderItem()
            => new Faker<Entities.OrderItem>()
            .CustomInstantiator(f => new Entities.OrderItem
            {
                Id       = f.Random.Guid(),
                Name     = f.Commerce.ProductName(),
                Price    = decimal.Parse(f.Commerce.Price()),
                Quantity = f.Random.Int(min: 1, max: 10)
            });
        
        public Entities.Customer NewCustomer()
            => new Faker<Entities.Customer>()
            .CustomInstantiator(f => new Entities.Customer
            {
                Id    = f.Random.Guid(),
                Name  = f.Person.FullName,
                Email = f.Person.Email
            });

        public Entities.CatalogItem NewCatalogItem()
            => new Faker<Entities.CatalogItem>()
            .CustomInstantiator(f => new Entities.CatalogItem
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