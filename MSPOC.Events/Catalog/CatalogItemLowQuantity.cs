using System;

namespace MSPOC.Events.Catalog
{
    public record CatalogItemLowQuantity(Guid ItemId, int Quantity);
}
