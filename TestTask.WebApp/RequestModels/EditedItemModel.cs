using System;
using System.ComponentModel.DataAnnotations;

namespace TestTask.Requests
{
    public class EditedItemModel
    {
        public Guid EditedItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        [StringLength(30)]
        public string Category { get; set; }
    }
}
