using MSPOC.CrossCutting;

namespace MSPOC.Order.Service.Entities
{
    public class CatalogItem : Entity
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}