using System;
using MSPOC.CrossCutting;

namespace MSPOC.Customer.Service.Entities
{
    public class Customer : Entity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string DocumentNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string CellNumber { get; set; }

        public CustomerAddress Address { get; set; }
    }
}