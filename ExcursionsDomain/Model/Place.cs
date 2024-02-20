using System;
using System.Collections.Generic;

namespace ExcursionsDomain.Model;

public partial class Place:Entity
{
    public string Name { get; set; } = null!;

    public string Adress { get; set; } = null!;

    public string? CoordinateX { get; set; }

    public string? CoordinateY { get; set; }

    public int CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Excursion> Excursions { get; set; } = new List<Excursion>();
}
