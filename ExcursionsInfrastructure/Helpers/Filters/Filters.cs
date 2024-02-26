using ExcursionsDomain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExcursionsInfrastructure.Helpers.Filters
{
    public class Filters
    {
        Dictionary<string, string> durations = new Dictionary<string, string>();
        Dictionary<string, string> excur_groups = new Dictionary<string, string>();

        public Filters() {
            durations["1"] = "0-5";
            durations["2"] = "6-24";
            excur_groups["1"] = "0-10";
            excur_groups["2"] = "11-40";
        }

        public IQueryable<Excursion> FilterExcursionsByDuration(IQueryable<Excursion> excursions, string selectedDuration)
        {
            if (!string.IsNullOrEmpty(selectedDuration))
            {
                if (selectedDuration == "3")
                {
                    excursions = excursions.Where(e => e.Duration > 24);
                }
                else
                {
                    var duration = durations[selectedDuration].Split('-').Select(int.Parse).ToArray();
                    excursions = excursions.Where(e => e.Duration >= duration[0] && e.Duration <= duration[1]);
                }
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByPeopleAmount(IQueryable<Excursion> excursions, string selectedPeopleAm)
        {
            if (!string.IsNullOrEmpty(selectedPeopleAm))
            {
                if (selectedPeopleAm == "3")
                {
                    excursions = excursions.Where(e => e.MaxPeopleAmount > 40);
                }
                else
                {
                    var group = excur_groups[selectedPeopleAm].Split('-').Select(int.Parse).ToArray();
                    excursions = excursions.Where(e => e.MaxPeopleAmount >= group[0] && e.MaxPeopleAmount <= group[1]);
                }
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByCategories(IQueryable<Excursion> excursions, int[] selectedCategories)
        {
            if (selectedCategories != null && selectedCategories.Any())
            {
                excursions = excursions.Where(e => e.Categories.Any(c => selectedCategories.Contains(c.Id)));
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByPlaces(IQueryable<Excursion> excursions, int[] selectedPlaces)
        {
            if (selectedPlaces != null && selectedPlaces.Any())
            {
                excursions = excursions.Where(e => e.Places.Any(p => selectedPlaces.Contains(p.Id)));
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByCities(IQueryable<Excursion> excursions, int[] selectedCities)
        {
            if (selectedCities != null && selectedCities.Any())
            {
                excursions = excursions.Where(e => e.Places.Any(p => selectedCities.Contains(p.CityId)));
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByCountries(IQueryable<Excursion> excursions, int[] selectedCountries)
        {
            if (selectedCountries != null && selectedCountries.Any())
            {
                excursions = excursions.Where(e => e.Places.Any(p => selectedCountries.Contains(p.City.CountryId)));
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByPrice(IQueryable<Excursion> excursions, string selectedPrices)
        {
            if (!string.IsNullOrEmpty(selectedPrices))
            {
                var priceRange = selectedPrices.Split('-').Select(s => int.Parse(s)).ToArray();
                excursions = excursions.Where(e => e.Price >= priceRange[0] && e.Price <= priceRange[1]);
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByDate(IQueryable<Excursion> excursions, DateTime dateFilter)
        {
            if (dateFilter != DateTime.MinValue)
            {
                excursions = excursions.Where(e => e.Date.Date == dateFilter.Date);
            }
            return excursions;
        }

        public IQueryable<Excursion> FilterExcursionsByName(IQueryable<Excursion> excursions, string nameFilter)
        {
            if (!string.IsNullOrEmpty(nameFilter))
            {
                excursions = excursions.Where(e => e.Name.Contains(nameFilter));
            }
            return excursions;
        }
    }
}
