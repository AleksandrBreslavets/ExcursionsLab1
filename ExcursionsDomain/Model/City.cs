using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class City : Entity
{

    [Required(ErrorMessage = "Назва міста є обов'язковою!")]
    [MaxLength(50, ErrorMessage = "Введіть коротшу назву міста")]
    [Display(Name = "Місто")]
    public string Name { get; set; } = null!;

    [Display(Name = "Країна")]
    [Required(ErrorMessage = "Оберіть країну!")]
    public int CountryId { get; set; }

    [Display(Name = "Країна")]
    public virtual Country Country { get; set; } = null!;

    [Display(Name = "Місця проведення")]
    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
