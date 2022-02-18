using System;

namespace MSPOC.CrossCutting
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }
}