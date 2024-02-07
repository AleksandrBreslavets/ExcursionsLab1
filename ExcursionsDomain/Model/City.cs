using System;
using System.Collections.Generic;

namespace ExcursionsDomain.Model;

public partial class City : Entity
{
    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
