using System.ComponentModel.DataAnnotations;

namespace ExcursionsInfrastructure.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введіть старий пароль.")]
        [Display(Name = "Старий пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Введіть новий пароль.")]
        [Display(Name = "Новий пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
