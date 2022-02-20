using System;
using System.Collections.Generic;
using MSPOC.CrossCutting;

namespace MSPOC.Customer.Service.Entities
{
    public class OrderHistory : Entity
    {
        public Guid CustomerId { get; set; }

        public Guid OrderId { get; set; }

        public string Description { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTimeOffset OrderedDate { get; set; }

        public DateTimeOffset DeliveryDate { get; set; }
    }
}