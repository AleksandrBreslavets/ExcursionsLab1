using System;
using System.Collections.Generic;

namespace ExcursionsDomain.Model;

public partial class Excursion:Entity
{
    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = null!;

    public int MaxPeopleAmount { get; set; }

    public double Price { get; set; }

    public int Duration { get; set; }

    public virtual ICollection<PlacesExcursion> PlacesExcursions { get; set; } = new List<PlacesExcursion>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();
}
