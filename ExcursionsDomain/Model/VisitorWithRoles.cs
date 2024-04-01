using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcursionsDomain.Model
{
    public class VisitorWithRoles
    {
        public Visitor Visitor { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public IList<string> Roles { get; set; } = null!;
    }
}
