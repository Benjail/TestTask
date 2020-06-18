using System;

namespace TestTask.Models
{
    public class OrderElement
    {
        public OrderElement(Guid ItemId, uint ItemCount)
        {
            this.ItemId = ItemId;
            this.ItemCount = ItemCount;
        }
        public Guid OrderElementId { get; set; }
        public Guid ItemId { get; set; }
        public uint ItemCount { get; set; }
        public float ItemPrice { get; set; } 
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}
