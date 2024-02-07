using System;
using System.Collections.Generic;

namespace ExcursionsDomain.Model;

public partial class Category : Entity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Excursion> Excursions { get; set; } = new List<Excursion>();
}
