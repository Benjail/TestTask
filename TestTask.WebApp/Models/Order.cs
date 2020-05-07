using System;
using System.Collections.Generic;

namespace TestTask.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipmentDate { get; set; }
        public float OrderNumber { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<OrderElement> OrderElements { get; set; }

    }
}
