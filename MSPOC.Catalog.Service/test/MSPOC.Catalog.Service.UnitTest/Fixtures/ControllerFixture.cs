using Bogus;
using MSPOC.Catalog.Service.Models;

namespace MSPOC.Catalog.Service.UnitTest.Fixtures
{
    public class ControllerFixture
    {
        public CreateEditItemDTO NewCreateEditItemDTO()
            => new Faker<CreateEditItemDTO>()
            .CustomInstantiator(f => new CreateEditItemDTO
            (
                Name        : f.Commerce.ProductName(),
                Description : f.Commerce.ProductDescription(),
                Price       : decimal.Parse(f.Commerce.Price()),
                Quantity    : f.Random.Int(min: 1, max: 10)
            ));

        public Entities.Item NewCatalogItem()
            => new Faker<Entities.Item>()
            .CustomInstantiator(f => new Entities.Item
            {
                Id             = f.Random.Guid(),
                Name           = f.Person.FullName,
                Description    = f.Commerce.ProductDescription(),
                Price          = decimal.Parse(f.Commerce.Price()),
                Quantity       = f.Random.Int(min:1, max: 10)
            });        
    }
}