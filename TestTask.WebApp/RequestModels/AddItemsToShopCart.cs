using System;

namespace TestTask.Requests
{
    public class AddItemsToShopCart
    {
        public Guid ItemId { get; set; }       
        public uint ItemCount { get; set; }
    }
}
