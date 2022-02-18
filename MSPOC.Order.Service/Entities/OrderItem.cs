using MSPOC.CrossCutting;

namespace MSPOC.Order.Service.Entities
{
    public class OrderItem : Entity
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}