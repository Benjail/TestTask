using System.ComponentModel.DataAnnotations;

namespace TestTask.Requests
{
    public class AddUserModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public float Discount { get; set; }
    }
}
