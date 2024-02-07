using System;
using System.Collections.Generic;

namespace ExcursionsDomain.Model;

public partial class Country:Entity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
