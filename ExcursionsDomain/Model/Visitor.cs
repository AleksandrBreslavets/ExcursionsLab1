using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class Visitor:Entity
{
    [Required(ErrorMessage = "Ім'я є обов'язковим!")]
    [MaxLength(50, ErrorMessage = "Введіть коротше ім'я")]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "E-mail є обов'язковим!")]
    [EmailAddress(ErrorMessage = "Невірний формат E-mail")]
    public string Email { get; set; } = null!;

    [Display(Name = "Номер телефону")]
    [Required(ErrorMessage = "Номер телефону є обов'язковим!")]
    [RegularExpression(@"^\+(?:[0-9] ?){6,14}[0-9]$", ErrorMessage = "Телефон має починатися з +, мати код країни та від 6 до 14 цифр.")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Екскурсії")]
    public virtual ICollection<Excursion> Excursions { get; set; } = new List<Excursion>();
}
