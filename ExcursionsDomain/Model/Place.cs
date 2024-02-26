using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class Place:Entity
{
    [Required(ErrorMessage = "Назва місця є обов'язковою!")]
    [MaxLength(50, ErrorMessage = "Введіть коротшу назву місця")]
    [Display(Name = "Місце")]
    public string Name { get; set; } = null!;

    [Display(Name = "Адреса")]
    [Required(ErrorMessage = "Адреса є обов'язковою!")]
    public string Adress { get; set; } = null!;

    [Display(Name = "Координата Х")]
    [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Неправильний формат координати")]
    public string? CoordinateX { get; set; }

    [Display(Name = "Координата Y")]
    [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Неправильний формат координати")]
    public string? CoordinateY { get; set; }

    [Required(ErrorMessage = "Місто є обов'язковим!")]
    [Display(Name = "Місто")]
    public int CityId { get; set; }

    [Display(Name = "Місто")]
    public virtual City City { get; set; } = null!;

    [Display(Name = "Екскурсії")]
    public virtual ICollection<Excursion> Excursions { get; set; } = new List<Excursion>();
}
