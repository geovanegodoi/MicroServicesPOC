using MSPOC.Events.Catalog;

namespace MSPOC.Catalog.Service.UnitTest.Extensions
{
    public static class Extensions
    {
        public static CatalogItemCreated AsItemCreated(this Entities.Item item)
            => new CatalogItemCreated(ItemId: item.Id, Name: item.Name, Price : item.Price);

        public static CatalogItemUpdated AsItemUpdated(this Entities.Item item)
            => new CatalogItemUpdated(ItemId: item.Id, Name: item.Name, Price : item.Price);

        public static CatalogItemDeleted AsItemDeleted(this Entities.Item item)
            => new CatalogItemDeleted(ItemId: item.Id);
    }
}