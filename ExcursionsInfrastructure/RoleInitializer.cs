using ExcursionsDomain.Model;
using ExcursionsInfrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace ExcursionsInfrastructure
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ExcursionsDbContext context)
        {
            string adminEmail = "admin@gmail.com";
            string password = "Sasha#123";
            string name = "Admin";
            string phone = "+380970000000";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail, Name=name, PhoneNumber=phone };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    Visitor v = new Visitor { Email = adminEmail, Name = name, PhoneNumber = phone };
                    context.Add(v);
                    await context.SaveChangesAsync();
                }
            }
        }

    }

}
