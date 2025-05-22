using System;

namespace GoDataSyncServices.Models
{
    public class AssemblyChildren
    {
        public Guid Id { get; set; }
        public Guid AssemblyId { get; set; }
        public Guid ChildAssemblyId { get; set; }
        public double Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 