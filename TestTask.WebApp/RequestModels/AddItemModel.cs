using System.ComponentModel.DataAnnotations;

namespace TestTask.Requests
{
    public class AddItemModel
    {
        public string Name { get; set; }
        public float Price { get; set; }
        [StringLength(30)]
        public string Category { get; set; }
        public string Code { get; set; }
    }
}
