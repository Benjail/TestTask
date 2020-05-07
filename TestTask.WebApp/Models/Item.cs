using System;
using System.ComponentModel.DataAnnotations;

namespace TestTask.Models
{
    public class Item
    {
        public Guid ItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        [StringLength(30)]
        public string Category { get; set; }
    }
}
