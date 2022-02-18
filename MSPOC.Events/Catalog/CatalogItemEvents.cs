using System;

namespace MSPOC.Events.Catalog
{
    public record CatalogItemCreated(Guid ItemId, string Name, decimal Price);
}
