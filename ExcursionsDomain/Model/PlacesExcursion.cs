using System;
using System.Collections.Generic;

namespace ExcursionsDomain.Model;

public partial class PlacesExcursion:Entity
{
    public int PlaceId { get; set; }

    public int ExcursionId { get; set; }

    public int? OrderNumber { get; set; }

    public virtual Excursion Excursion { get; set; } = null!;

    public virtual Place Place { get; set; } = null!;
}
