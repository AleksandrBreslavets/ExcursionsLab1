using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class Category : Entity
{
    [Required(ErrorMessage ="Поле має бути заповненим!")]
    [Display(Name="Категорія")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Excursion> Excursions { get; set; } = new List<Excursion>();
}
