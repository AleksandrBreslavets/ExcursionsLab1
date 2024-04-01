using Microsoft.AspNetCore.Identity;

namespace ExcursionsInfrastructure.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
