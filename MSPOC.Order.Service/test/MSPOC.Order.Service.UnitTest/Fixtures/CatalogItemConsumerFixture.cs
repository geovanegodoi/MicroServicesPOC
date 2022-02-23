using Bogus;
using MSPOC.Events.Catalog;
using MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.UnitTest.Fixtures
{
    public class CatalogItemConsumerFixture
    {
        public CatalogItem NewCatalogItem()
            => new Faker<CatalogItem>()
            .CustomInstantiator(f => new CatalogItem
            {
                Id    = f.Random.Guid(),
                Name  = f.Commerce.ProductName(),
                Price = decimal.Parse(f.Commerce.Price())
            });

        public CatalogItemCreated NewCatalogItemCreated()
            => new Faker<CatalogItemCreated>()
            .CustomInstantiator(f => new CatalogItemCreated
            (
                ItemId : f.Random.Guid(), 
                Name   : f.Commerce.ProductName(),
                Price  : f.Random.Decimal()
            ));

        public CatalogItemDeleted NewCatalogItemDeleted()
            => new Faker<CatalogItemDeleted>()
            .CustomInstantiator(f => new CatalogItemDeleted 
            ( 
                ItemId : f.Random.Guid() 
            ));

        public CatalogItemUpdated NewCatalogItemUpdated()
            => new Faker<CatalogItemUpdated>()
            .CustomInstantiator(f => new CatalogItemUpdated
            (
                ItemId : f.Random.Guid(), 
                Name   : f.Commerce.ProductName(),
                Price  : f.Random.Decimal()
            ));
    }
}