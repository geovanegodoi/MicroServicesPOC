using MassTransit;
using MSPOC.Events.Catalog;
using MSPOC.CrossCutting;

namespace MSPOC.Catalog.Service.Entities
{
    public class Item : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}