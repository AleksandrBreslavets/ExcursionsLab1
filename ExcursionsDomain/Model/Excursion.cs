using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class Excursion:Entity
{
    [Display(Name = "Час проведення")]
    [Required(ErrorMessage = "Дата та час є обов'язковими!")]
    public DateTime Date { get; set; }

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Назва є обов'язковою!")]
    [MaxLength(50, ErrorMessage = "Введіть коротшу назву")]
    public string Name { get; set; } = null!;

    [Display(Name = "Максимальна кількість відвідувачів")]
    [Required(ErrorMessage = "Максимальна кількість відвідувачів обов'язкова!")]
    [Range(1, int.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Максимальна кількість людей повинна бути цілим додатнім числом")]
    public int MaxPeopleAmount { get; set; }

    [Display(Name = "Ціна")]
    [Required(ErrorMessage = "Ціна є обов'язковою!")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Ціна повинна бути додатнім десятковим числом з максимально двома знаками після крапки")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
    public double Price { get; set; }

    [Display(Name = "Тривалість")]
    [Required(ErrorMessage = "Тривалість є обов'язковою!")]
    [Range(1, int.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Тривалість повинна бути цілим додатнім числом")]
    public int Duration { get; set; }

    [Display(Name = "Місця проведення")]
    [Required(ErrorMessage = "Оберіть принаймні одне місце проведення!")]
    public virtual ICollection<Place> Places { get; set; } = new List<Place>();

    [Display(Name = "Категорії")]
    [Required(ErrorMessage = "Оберіть принаймні одну категорію!")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    [Display(Name = "Відвідувачі")]
    public virtual ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
}
