using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExcursionsDomain.Model;
using ExcursionsInfrastructure;
using Microsoft.AspNetCore.Authorization;
using ExcursionsInfrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ExcursionsInfrastructure.Controllers
{
    public class VisitorsController : Controller
    {
        private readonly ExcursionsDbContext _context;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public VisitorsController(ExcursionsDbContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        // GET: Visitors
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles =  _userManager.Users.Select(user => new
            {
                User = user.UserName,
                UserId=user.Id,
                Roles = _userManager.GetRolesAsync(user).Result
            }).ToList();

            var visitorsWithRoles = _context.Visitors
                .AsEnumerable() 
                .Join(
                    usersWithRoles,
                    visitor => visitor.Email,
                    other => other.User,
                    (visitor, other) => new VisitorWithRoles
                    {
                        Visitor = visitor,
                        UserId=other.UserId,
                        Roles = other.Roles
                    }
                )
                .ToList();

            return View(visitorsWithRoles);
        }

        // GET: Visitors/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .Include(v=>v.Excursions)
                .ThenInclude(pl => pl.Places)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitor == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(visitor.Email);
            var roles = await _userManager.GetRolesAsync(user);

            var visitorsWithRoles = new VisitorWithRoles { Visitor = visitor, Roles=roles, UserId=user.Id };
            return View(visitorsWithRoles);
        }

        private bool VisitorExists(int id)
        {
            return _context.Visitors.Any(e => e.Id == id);
        }

        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddExcursion(int excur_id)
        {
            var visitor = _context.Visitors.Include(v=>v.Excursions).FirstOrDefault(v => v.Email == User.Identity.Name);
            

            if (visitor == null)
            {
                return NotFound();
            }

            var excursion = _context.Excursions.FirstOrDefault(e => e.Id == excur_id);

            if (excursion == null)
            {
                return NotFound();
            }

            visitor.Excursions.Add(excursion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Excursions");
        }

        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteExcursion(int excur_id)
        {
            var visitor = _context.Visitors.Include(v => v.Excursions).FirstOrDefault(v => v.Email == User.Identity.Name);


            if (visitor == null)
            {
                return NotFound();
            }

            var excursion = _context.Excursions.FirstOrDefault(e => e.Id == excur_id);

            if (excursion == null)
            {
                return NotFound();
            }

            visitor.Excursions.Remove(excursion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Excursions");
        }
    }
}
