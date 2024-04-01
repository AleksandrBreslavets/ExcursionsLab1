using System.ComponentModel.DataAnnotations;

namespace ExcursionsInfrastructure.ViewModel
{
    public class UpdateUserViewModel
    {
        [Required(ErrorMessage = "Введіть E-mail.")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введіть номер телефону.")]
        [RegularExpression(@"^\+(?:[0-9] ?){6,14}[0-9]$", ErrorMessage = "Телефон має починатися з +, мати код країни та від 6 до 14 цифр.")]
        [Display(Name = "Номер телефону")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Введіть ваше ім'я.")]
        [MaxLength(50, ErrorMessage = "Введіть коротше ім'я")]
        [Display(Name = "Ім'я")]
        public string Name { get; set; }
    }
}
