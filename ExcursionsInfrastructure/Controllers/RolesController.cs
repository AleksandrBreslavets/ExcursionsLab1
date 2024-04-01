using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using ExcursionsInfrastructure.Models;
using ExcursionsInfrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExcursionsInfrastructure.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index() => View(_roleManager.Roles.ToList());

        public async Task<IActionResult> Edit(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                if (roles.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "Оберіть принаймні одну роль.");
                    ChangeRoleViewModel model = new ChangeRoleViewModel
                    {
                        UserId = user.Id,
                        UserEmail = user.Email,
                        UserRoles = userRoles,
                        AllRoles = allRoles
                    };
                    return View(model);
                }

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("Index", "Visitors");
            }

            return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] AddRoleViewModel r)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.FindByNameAsync(r.Name) == null)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(r.Name));
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }  
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Така роль вже існує.");
                }
            }
            return View(r);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string roleId)
        {
            if (roleId.IsNullOrEmpty())
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            if((role.Name=="admin") || (role.Name == "user"))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string roleId)
        {
            if (roleId.IsNullOrEmpty())
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            if ((role.Name == "admin") || (role.Name == "user"))
            {
                return RedirectToAction(nameof(Index));
            }

            var usersWithRole = await _userManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in usersWithRole)
            {
                var resultRem = await _userManager.RemoveFromRoleAsync(user, role.Name);
                if (!resultRem.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(string roleId)
        {
            if (roleId.IsNullOrEmpty())
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            if ((role.Name == "admin") || (role.Name == "user"))
            {
                return RedirectToAction(nameof(Index));
            } 
          
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("Name, Id")] IdentityRole role)
        {
            if (role.Name.IsNullOrEmpty())
            {
                ModelState.AddModelError(string.Empty, "Введіть назву ролі.");
            }

            if (ModelState.IsValid)
            {
                if (role.Id.IsNullOrEmpty())
                {
                    return NotFound();
                }
                var roleToUpdate = await _roleManager.FindByIdAsync(role.Id);

                if (roleToUpdate == null)
                {
                    return NotFound();
                }

                if ((roleToUpdate.Name == "admin") || (roleToUpdate.Name == "user"))
                {
                    return RedirectToAction(nameof(Index));
                }

                string oldRole=roleToUpdate.Name;
                roleToUpdate.Name = role.Name;

                var result = await _roleManager.UpdateAsync(roleToUpdate);
                if (result.Succeeded)
                {
                    var usersWithRole = await _userManager.GetUsersInRoleAsync(oldRole);
                    foreach (var user in usersWithRole)
                    {
                        await _userManager.RemoveFromRoleAsync(user, oldRole);
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(role);
        }

    }

}
