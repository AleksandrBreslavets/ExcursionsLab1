using System;
using System.Collections.Generic;

namespace ExcursionsDomain.Model;

public partial class Visitor:Entity
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Excursion> Excursions { get; set; } = new List<Excursion>();
}
