using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExcursionsDomain.Model;
using ExcursionsInfrastructure;

namespace ExcursionsInfrastructure.Controllers
{
    public class PlacesExcursionsController : Controller
    {
        private readonly ExcursionsDbContext _context;

        public PlacesExcursionsController(ExcursionsDbContext context)
        {
            _context = context;
        }

        // GET: PlacesExcursions
        public async Task<IActionResult> Index()
        {
            var excursionsDbContext = _context.PlacesExcursions.Include(p => p.Excursion).Include(p => p.Place);
            return View(await excursionsDbContext.ToListAsync());
        }

        // GET: PlacesExcursions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placesExcursion = await _context.PlacesExcursions
                .Include(p => p.Excursion)
                .Include(p => p.Place)
                .FirstOrDefaultAsync(m => m.PlaceId == id);
            if (placesExcursion == null)
            {
                return NotFound();
            }

            return View(placesExcursion);
        }

        // GET: PlacesExcursions/Create
        public IActionResult Create()
        {
            ViewData["ExcursionId"] = new SelectList(_context.Excursions, "Id", "Name");
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Adress");
            return View();
        }

        // POST: PlacesExcursions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaceId,ExcursionId,OrderNumber")] PlacesExcursion placesExcursion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(placesExcursion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExcursionId"] = new SelectList(_context.Excursions, "Id", "Name", placesExcursion.ExcursionId);
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Adress", placesExcursion.PlaceId);
            return View(placesExcursion);
        }

        // GET: PlacesExcursions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placesExcursion = await _context.PlacesExcursions.FindAsync(id);
            if (placesExcursion == null)
            {
                return NotFound();
            }
            ViewData["ExcursionId"] = new SelectList(_context.Excursions, "Id", "Name", placesExcursion.ExcursionId);
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Adress", placesExcursion.PlaceId);
            return View(placesExcursion);
        }

        // POST: PlacesExcursions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlaceId,ExcursionId,OrderNumber")] PlacesExcursion placesExcursion)
        {
            if (id != placesExcursion.PlaceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(placesExcursion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlacesExcursionExists(placesExcursion.PlaceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExcursionId"] = new SelectList(_context.Excursions, "Id", "Name", placesExcursion.ExcursionId);
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Adress", placesExcursion.PlaceId);
            return View(placesExcursion);
        }

        // GET: PlacesExcursions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placesExcursion = await _context.PlacesExcursions
                .Include(p => p.Excursion)
                .Include(p => p.Place)
                .FirstOrDefaultAsync(m => m.PlaceId == id);
            if (placesExcursion == null)
            {
                return NotFound();
            }

            return View(placesExcursion);
        }

        // POST: PlacesExcursions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var placesExcursion = await _context.PlacesExcursions.FindAsync(id);
            if (placesExcursion != null)
            {
                _context.PlacesExcursions.Remove(placesExcursion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlacesExcursionExists(int id)
        {
            return _context.PlacesExcursions.Any(e => e.PlaceId == id);
        }
    }
}
