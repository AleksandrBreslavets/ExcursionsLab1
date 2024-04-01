using DocumentFormat.OpenXml.InkML;
using ExcursionsDomain.Model;
using ExcursionsInfrastructure.Models;
using ExcursionsInfrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ExcursionsInfrastructure.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ExcursionsDbContext _excursionsDbContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ExcursionsDbContext excursionsDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _excursionsDbContext = excursionsDbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, PhoneNumber = model.PhoneNumber, Name = model.Name };
                List<string> roles = new List<string>();
                roles.Add("user");

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, roles);
                    await _signInManager.SignInAsync(user, false);

                    Visitor v = new Visitor { Email = user.Email, PhoneNumber = user.PhoneNumber, Name = user.Name };
                    _excursionsDbContext.Add(v);
                    await _excursionsDbContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильний логін чи (та) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details()
        {
            var visitor = await _excursionsDbContext.Visitors
                .Include(v => v.Excursions)
                .ThenInclude(pl => pl.Places)
                .FirstOrDefaultAsync(m => m.Email == User.Identity.Name);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.OldPassword);

                    if (isPasswordCorrect)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Details", "Account");
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
                        ModelState.AddModelError("", "Старий пароль неправильний.");
                    }
                }
                else
                {
                    return NotFound();
                }


            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> UpdateUser()
        {
            string userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var visitor = _excursionsDbContext.Visitors.FirstOrDefault(v => v.Email == userName);
            if (user == null || visitor==null)
            {
                return NotFound();
            }
            UpdateUserViewModel u=new UpdateUserViewModel { PhoneNumber=user.PhoneNumber, Email=user.Email, Name=user.Name};
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> UpdateUser([Bind("Name, Email, PhoneNumber")] UpdateUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                string userName = User.Identity.Name;
                var userToUpdate = await _userManager.FindByNameAsync(userName);
                var visitor = _excursionsDbContext.Visitors.FirstOrDefault(v => v.Email == userName);

                if (user == null || visitor == null)
                {
                    return NotFound();
                }

                userToUpdate.Name= user.Name;
                userToUpdate.Email= user.Email;
                userToUpdate.UserName= user.Email;
                userToUpdate.PhoneNumber= user.PhoneNumber;

                visitor.Name = user.Name;
                visitor.Email = user.Email;
                visitor.PhoneNumber = user.PhoneNumber;

                var result = await _userManager.UpdateAsync(userToUpdate);
                if (result.Succeeded)
                {
                    try
                    {
                        _excursionsDbContext.Update(visitor);
                        await _excursionsDbContext.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    await _signInManager.SignInAsync(userToUpdate, false);
                    return RedirectToAction("Details", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            if (userName.IsNullOrEmpty())
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(userName);

            var visitor = _excursionsDbContext.Visitors.FirstOrDefault(v => v.Email == userName);
            if (user == null || visitor == null)
            {
                return NotFound();
            }
            
            return View(visitor);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteUserConfirmed(string userName)
        {
            if (userName.IsNullOrEmpty())
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(userName);
            var visitor = _excursionsDbContext.Visitors.FirstOrDefault(v => v.Email == userName);

            if (user == null || visitor == null)
            {
                return NotFound();
            }

            if (userName == User.Identity.Name)
            {
                await _signInManager.SignOutAsync();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                {
                _excursionsDbContext.Visitors.Remove(visitor);
                await _excursionsDbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
