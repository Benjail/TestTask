using System;

namespace TestTask.Requests
{
    public class ConfirmOderModel
    {
        public Guid OrderId { get; set; }
        public DateTime DeliveryDate { get; set; } 
    }
}
