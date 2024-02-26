using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExcursionsDomain.Model;
using ExcursionsInfrastructure;
using ExcursionsInfrastructure.Helpers.Filters;

namespace ExcursionsInfrastructure.Controllers
{
    public class ExcursionsController : Controller
    {
        private readonly ExcursionsDbContext _context;
        
        public ExcursionsController(ExcursionsDbContext context)
        {
            _context = context;
        }

        private List<SelectListItem> FormPricesDiapazones()
        {
            var maxPriceDouble =  _context.Excursions.Max(x => x.Price);

            var maxPriceInt = (int)Math.Ceiling(maxPriceDouble);

            var pricesList = new List<SelectListItem>();

            int step = 1000;

            for (int i = 0; i <= maxPriceInt; i += step)
            {
                int startRange = i;
                int endRange = Math.Min(i + step - 1, maxPriceInt);

                string rangeText = $"{startRange}-{endRange}";

                pricesList.Add(new SelectListItem
                {
                    Value = rangeText,
                    Text = rangeText
                });
            }
            return pricesList;
        }

        private IQueryable<Excursion> FilterExcursions(
            int[] selectedCategories,
            string nameFilter,
            int[] selectedPlaces,
            int[] selectedCities,
            int[] selectedCountries,
            string selectedPrices,
            DateTime dateFilter,
            string selectedDuration,
            string selectedPeopleAm)
        {
            IQueryable<Excursion> filteredExcursions = _context.Excursions;
            Filters filter = new Filters();

            filteredExcursions = filter.FilterExcursionsByCategories(filteredExcursions, selectedCategories);
            filteredExcursions = filter.FilterExcursionsByPlaces(filteredExcursions, selectedPlaces);
            filteredExcursions = filter.FilterExcursionsByCities(filteredExcursions, selectedCities);
            filteredExcursions = filter.FilterExcursionsByCountries(filteredExcursions, selectedCountries);
            filteredExcursions= filter.FilterExcursionsByPrice(filteredExcursions, selectedPrices);
            filteredExcursions= filter.FilterExcursionsByDate(filteredExcursions, dateFilter);
            filteredExcursions= filter.FilterExcursionsByName(filteredExcursions, nameFilter);
            filteredExcursions = filter.FilterExcursionsByDuration(filteredExcursions, selectedDuration);
            filteredExcursions = filter.FilterExcursionsByPeopleAmount(filteredExcursions, selectedPeopleAm);

            return filteredExcursions;
        }

        // GET: Excursions
        public async Task<IActionResult> Index(
            int[] selectedCategories, 
            string nameFilter, 
            int[] selectedPlaces, 
            int[] selectedCities, 
            int[] selectedCountries,
            string selectedPrices,
            DateTime dateFilter,
            string selectedDuration,
            string selectedPeopleAm)
        {
            await _context.Excursions
                .Include(e => e.Places)
                .ToListAsync();

            var filteredExcursions = FilterExcursions(
                selectedCategories,
                nameFilter,
                selectedPlaces,
                selectedCities,
                selectedCountries, 
                selectedPrices, 
                dateFilter, 
                selectedDuration, 
                selectedPeopleAm);

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Places"] = new SelectList(_context.Places, "Id", "Name");
            ViewData["Cities"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["Countries"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["Prices"] = FormPricesDiapazones();
            ViewData["Visitor"] = _context.Visitors.Include(v=>v.Excursions).FirstOrDefault(v => v.Id == 3);

            return View(await filteredExcursions.ToListAsync());
        }

        // GET: Excursions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excursion = await _context.Excursions
                .FirstOrDefaultAsync(m => m.Id == id);

            await _context.Excursions
                 .Include(e => e.Categories)
                 .ToListAsync();

            await _context.Excursions
                 .Include(pe => pe.Places)
                 .ThenInclude(pl=>pl.City)
                 .ToListAsync();
            if (excursion == null)
            {
                return NotFound();
            }

            ViewData["Visitor"] = _context.Visitors.Include(v => v.Excursions).FirstOrDefault(v => v.Id == 3);

            return View(excursion);
        }

        // GET: Excursions/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Places"] = new SelectList(_context.Places, "Id", "Name");
            return View();
        }

        private ICollection<T> GetEntities<T>(int[] ids) where T : Entity
        {
            ICollection<T> entities = new List<T>();
            foreach (int id in ids)
            {
                T entity = _context.Set<T>().FirstOrDefault(e => e.Id == id);
                entities.Add(entity);
            }

            return entities;
        }

        // POST: Excursions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Description,Name,MaxPeopleAmount,Price,Duration,Id")] Excursion excursion, int[] Categories, int[] Places)
        {
            if (ModelState.IsValid)
            {
                excursion.Categories= GetEntities<Category>(Categories);
                excursion.Places=GetEntities<Place>(Places);
                _context.Add(excursion);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Places"] = new SelectList(_context.Places, "Id", "Name");
            return View(excursion);
        }

        // GET: Excursions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _context.Excursions
                 .Include(e => e.Categories)
                 .ToListAsync();
            await _context.Excursions
                 .Include(e => e.Places)
                 .ToListAsync();

            var excursion = await _context.Excursions.FindAsync(id);
            if (excursion == null)
            {
                return NotFound();
            }

            ViewData["SelectedCategories"]= excursion.Categories.Select(c => c.Id).ToArray();
            ViewData["SelectedPlaces"] = excursion.Places.Select(c => c.Id).ToArray();
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Places"] = new SelectList(_context.Places, "Id", "Name");
            return View(excursion);
        }

        private ICollection<T> GetEntitiesEdit<T>(int[] ids, Excursion e) where T : Entity
        {
            ICollection<T> entities = new List<T>();
            foreach (int id in ids)
            {

                bool alreadyExists = false;
                if (typeof(T) == typeof(Place)) 
                {
                    alreadyExists = e.Places.Any(p => p.Id == id);
                }
                else if (typeof(T) == typeof(Category))
                {
                    alreadyExists = e.Categories.Any(c => c.Id == id);
                }

                if (!alreadyExists)
                {
                    T entity = _context.Set<T>().FirstOrDefault(e => e.Id == id);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        private void UpdateCategories(Excursion existingExcursion, int[] Categories)
        {
            var curCatIds = existingExcursion.Categories.Select(c => c.Id).ToList();
            var catToAdd = Categories.Except(curCatIds).ToList();
            var catToRemove = curCatIds.Except(Categories).ToList();

            foreach (var catId in catToRemove)
            {
                var cat = existingExcursion.Categories.FirstOrDefault(c => c.Id == catId);
                if (cat != null) existingExcursion.Categories.Remove(cat);
            }

            foreach (var catId in catToAdd)
            {
                var cat = _context.Categories.FirstOrDefault(c => c.Id == catId);
                existingExcursion.Categories.Add(cat);
            }
        }

        private void UpdatePlaces(Excursion existingExcursion, int[] Places)
        {
            var curPlIds = existingExcursion.Places.Select(p => p.Id).ToList();
            var plToAdd = Places.Except(curPlIds).ToList();
            var plToRemove = curPlIds.Except(Places).ToList();

            foreach (var plId in plToRemove)
            {
                var pl = existingExcursion.Places.FirstOrDefault(p => p.Id == plId);
                if (pl != null) existingExcursion.Places.Remove(pl);
            }

            foreach (var plId in plToAdd)
            {
                var pl = _context.Places.FirstOrDefault(p => p.Id == plId);
                existingExcursion.Places.Add(pl);
            }
        }

        // POST: Excursions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Date,Description,Name,MaxPeopleAmount,Price,Duration,Id")] Excursion excursion, int[] Categories, int[] Places)
        {
            if (id != excursion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(excursion);

                    var existingExcursion = await _context.Excursions
                        .Include(e => e.Categories)
                        .Include(e => e.Places)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existingExcursion == null)
                    {
                        return NotFound();
                    }

                    UpdateCategories(existingExcursion, Categories);
                    UpdatePlaces(existingExcursion, Places);
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExcursionExists(excursion.Id))
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

            return View(excursion);
        }
        // GET: Excursions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excursion = await _context.Excursions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (excursion == null)
            {
                return NotFound();
            }

            return View(excursion);
        }

        // POST: Excursions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var excursion = await _context.Excursions.FindAsync(id);
            
            if (excursion != null)
            {
                _context.Excursions.Remove(excursion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExcursionExists(int id)
        {
            return _context.Excursions.Any(e => e.Id == id);
        }

        [HttpPost, ActionName("Filter")]
        public async Task<IActionResult> Filter(
            int[] selectedCategories, 
            string nameFilter, 
            int[] selectedCities, 
            int[] selectedCountries, 
            int[] selectedPlaces, 
            string selectedPrices, 
            DateTime dateFilter, 
            string selectedDuration,
            string selectedPeopleAm)
            
        {
            object routeValues;

            if (dateFilter == DateTime.MinValue)
            {
                routeValues = new { selectedCategories, nameFilter, selectedPlaces, selectedCities, selectedCountries, selectedPrices, selectedDuration, selectedPeopleAm };
            }
            else
            {
                routeValues = new { selectedCategories, nameFilter, selectedPlaces, selectedCities, selectedCountries, selectedPrices, dateFilter , selectedDuration, selectedPeopleAm };
            }

            return RedirectToAction("Index", routeValues);
        }


    }
}
