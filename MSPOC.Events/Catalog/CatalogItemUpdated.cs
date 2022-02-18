using System;

namespace MSPOC.Events.Catalog
{
    public record CatalogItemUpdated(Guid ItemId, string Name, decimal Price);
}
