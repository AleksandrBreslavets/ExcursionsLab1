using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExcursionsDomain.Model;

public partial class Excursion:Entity
{
    [Display(Name = "Час проведення")]
    public DateTime Date { get; set; }

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Display(Name = "Назва")]
    public string Name { get; set; } = null!;

    [Display(Name = "Максимальна кількість людей")]
    public int MaxPeopleAmount { get; set; }

    [Display(Name = "Ціна")]
    public double Price { get; set; }

    [Display(Name = "Тривалість")]
    public int Duration { get; set; }

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
}
