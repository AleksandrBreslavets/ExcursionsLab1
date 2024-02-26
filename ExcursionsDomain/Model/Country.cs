using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class Country:Entity
{

    [Required(ErrorMessage = "Назва країни є обов'язковою!")]
    [MaxLength(50, ErrorMessage = "Введіть коротшу назву країни")]
    [Display(Name = "Країна")]
    public string Name { get; set; } = null!;

    [Display(Name = "Міста")]
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
