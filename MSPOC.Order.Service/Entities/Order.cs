using System;
using System.Collections.Generic;
using System.Linq;
using MSPOC.CrossCutting;

namespace MSPOC.Order.Service.Entities
{
    public class Order : Entity
    {
        public string Number { get; set; }

        public string Description { get; set; }

        public DateTimeOffset DeliveryDate { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; }

        public Customer Customer { get; set; }

        public decimal CalculateOrderTotalPrice()
            => OrderItems.Sum(item => item.Price * item.Quantity);
    }
}