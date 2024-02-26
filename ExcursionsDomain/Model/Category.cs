using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class Category : Entity
{
    [Required(ErrorMessage ="Назва категорії є обов'язковою!")]
    [MaxLength(50, ErrorMessage = "Введіть коротшу назву категорії")]
    [Display(Name="Категорія")]
    public string Name { get; set; } = null!;

    [Display(Name = "Екскурсії")]
    public virtual ICollection<Excursion> Excursions { get; set; } = new List<Excursion>();
}
