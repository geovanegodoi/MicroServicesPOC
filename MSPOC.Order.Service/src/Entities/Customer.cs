using MSPOC.CrossCutting;

namespace MSPOC.Order.Service.Entities
{
    public class Customer : Entity
    {
        public string Name { get; set; }
        
        public string Email { get; set; }
    }
}