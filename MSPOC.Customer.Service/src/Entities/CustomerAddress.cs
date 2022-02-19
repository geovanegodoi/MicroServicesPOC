using System;
using MSPOC.CrossCutting;

namespace MSPOC.Customer.Service.Entities
{
    public class CustomerAddress
    {
        public string Street { get; set; }

        public int Number { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
    }
}