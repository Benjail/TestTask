using System.ComponentModel.DataAnnotations;

namespace TestTask.Requests
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Не указано имя")] 
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан Email")] 
        public string Email { get; set; } 

        [Required(ErrorMessage = "Не указан пароль")] 
        public string Password { get; set; }
    }
}
