using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using ExcursionsDomain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using City = ExcursionsDomain.Model.City;

namespace ExcursionsInfrastructure.Services
{
    public class ExcursionImportService : IImportService<ExcursionsDomain.Model.Excursion>
    {
        private readonly ExcursionsDbContext _context;
        private static bool _isValid;
        private static string _validationErrorStr; 

        public ExcursionImportService(ExcursionsDbContext context)
        {
            _validationErrorStr = "";
            _isValid = true;
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    var catname = worksheet.Name;
                    var category = await _context.Categories.FirstOrDefaultAsync(cat => cat.Name == catname, cancellationToken);

                    if (category == null)
                    {
                        category = new ExcursionsDomain.Model.Category();
                        category.Name = catname;

                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                 
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        await AddExcursionAsync(row, cancellationToken, worksheet.Name);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private bool IsExcursInDB(Excursion e)
        {
            var excursion = _context.Excursions.FirstOrDefault(excur => excur.Name == e.Name
            && excur.Date == e.Date && excur.Duration == e.Duration && excur.MaxPeopleAmount == e.MaxPeopleAmount
            && excur.Price == e.Price);

            if(excursion == null)
            {
                return false;
            }
            
            return true;
        }

        private async Task AddExcursionAsync(IXLRow row, CancellationToken cancellationToken, string wName)
        {
            Excursion e = new Excursion();
            e.Date = GetExcurDate(row, wName);
            e.Description = GetExcurDescr(row);
            e.Name = GetExcurName(row, wName);
            e.MaxPeopleAmount = GetExcurNumberField(row, 3, wName);
            e.Price = GetExcurPrice(row, wName);
            e.Duration = GetExcurNumberField(row, 8, wName);

            if (_isValid)
            {
                if (!IsExcursInDB(e))
                {
                    _context.Excursions.Add(e);

                    await GetCategoriesAsync(row, e, cancellationToken);
                    await GetPlacesAsync(row, e, cancellationToken, wName);
                    if (!_isValid)
                    {
                        throw new ArgumentException(_validationErrorStr);
                    }
                } 
            }
            else
            {
                throw new ArgumentException(_validationErrorStr);
            }
            
        }

        private static DateTime GetExcurDate(IXLRow row, string wName)
        {
            int rowNumber = row.RowNumber();
            string dateStr = row.Cell(2).Value.ToString();

            if (string.IsNullOrEmpty(dateStr))
            {
                _isValid = false;
                _validationErrorStr += $"Дата та час екскурсії в рядку: {rowNumber} обов'язкові.\n";
                return DateTime.MinValue;
            }

            DateTime date;

            if (DateTime.TryParse(dateStr, out date))
            {
                if (date < DateTime.Now)
                {
                    _isValid = false;
                    _validationErrorStr += $"Дата та час екскурсії в категорії: {wName}, в рядку: {rowNumber} не можуть бути раніше поточного часу.\n";
                    return DateTime.MinValue;
                }

                return date;
            }
            else
            {
                _isValid = false;
                _validationErrorStr += $"Помилка в форматі дати та часу екскурсії в категорії: {wName}, в рядку: {rowNumber}.\n";
                return DateTime.MinValue;
            }
        }

        private static string GetExcurDescr(IXLRow row)
        {
            return row.Cell(6).Value.ToString();
        }

        private static string GetExcurName(IXLRow row, string wName)
        {
            int rowNumber = row.RowNumber();
            string value = row.Cell(1).Value.ToString();

            if (string.IsNullOrEmpty(value))
            {
                _isValid = false;
                _validationErrorStr += $"Назва екскурсії в категорії: {wName}, в рядку: {rowNumber} обов'язкова.\n";
                return "";
            }
            if (value.Length > 50)
            {
                _isValid = false;
                _validationErrorStr += $"Назва екскурсії в категорії: {wName}, в рядку: {rowNumber} занадто довга.\n";
                return "";
            }
            return value;
        }

        private static int GetExcurNumberField(IXLRow row, int cellNumb, string wName)
        {
            int rowNumber = row.RowNumber();
            string valueStr = row.Cell(cellNumb).Value.ToString();

            if (string.IsNullOrEmpty(valueStr))
            {
                _isValid = false;
                _validationErrorStr += $"Числове значення в категорії: {wName}, в рядку: {rowNumber}, клітинка: {cellNumb} обов'язкове.\n";
                return 0;
            }

            int value;

            if (int.TryParse(valueStr, out value))
            {
                if(value < 0)
                {
                    _isValid = false;
                    _validationErrorStr += $"Числове значення в категорії: {wName}, в рядку: {rowNumber}, клітинка: {cellNumb} має бути додатнім.\n";
                    return 0;
                }
                Regex regex = new Regex(@"^\d+$");
                if (!regex.IsMatch(valueStr))
                {
                    _isValid = false;
                    _validationErrorStr += $"Помилка в форматі числового значення в категорії: {wName}, в рядку: {rowNumber}, клітинка: {cellNumb}.\n";
                    return 0;
                }
                return value;
            }
            else
            {
                _isValid = false;
                _validationErrorStr += $"Помилка в форматі числового значення в категорії: {wName}, в рядку: {rowNumber}, клітинка: {cellNumb}.\n";
                return 0;
            }
        }

        private static double GetExcurPrice(IXLRow row, string wName)
        {
            int rowNumber = row.RowNumber();
            string valueStr = row.Cell(7).Value.ToString();

            if (string.IsNullOrEmpty(valueStr))
            {
                _isValid = false;
                _validationErrorStr += $"Ціна екскурсії в категорії: {wName}, в рядку: {rowNumber} обов'язкове.\n";
                return 0;
            }

            double value;

            if (double.TryParse(valueStr, out value))
            {
                if (value < 0.01)
                {
                    _isValid = false;
                    _validationErrorStr += $"Ціна екскурсії в категорії: {wName}, в рядку: {rowNumber} має бути додатньою.\n";
                    return 0;
                }

                Regex regex = new Regex(@"^\d+(\.\d{1,2})?$");
                if (!regex.IsMatch(valueStr))
                {
                    _isValid = false;
                    _validationErrorStr += $"Помилка в форматі ціни екскурсії в категорії: {wName}, в рядку: {rowNumber}.\n";
                    return 0;
                }

                return value;
            }
            else
            {
                _isValid = false;
                _validationErrorStr += $"Помилка в форматі ціни екскурсії в категорії: {wName}, в рядку: {rowNumber}.\n";
                return 0;
            }
        }

        private async Task GetCategoriesAsync(IXLRow row, Excursion e, CancellationToken cancellationToken)
        {
            string value = row.Cell(4).Value.ToString();

            if(!string.IsNullOrEmpty(value))
            {
                var categoriesArr = value.Split(",").Select(part => part.Trim()).ToArray();

                for (int i = 0; i < categoriesArr.Length; i++)
                {
                    var categoryName = categoriesArr[i];
                    var category = await _context.Categories.FirstOrDefaultAsync(cat => cat.Name == categoryName, cancellationToken);

                    if (category is null)
                    {
                        category = new ExcursionsDomain.Model.Category();
                        category.Name = categoryName;
                        _context.Categories.Add(category);
                    }

                    e.Categories.Add(category);
                }
            }
            
        }

        private async Task GetCity(Place p, string cityCountrySubstring, CancellationToken cancellationToken)
        {

            string[] cityCountry = cityCountrySubstring.Split(',').Select(part => part.Trim()).ToArray();

            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == cityCountry[1], cancellationToken);
            if (country is null)
            {
                country = new Country();
                country.Name = cityCountry[1];
            }

            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == cityCountry[0], cancellationToken);
            if (city is null)
            {
                city = new City();
                city.Name = cityCountry[0];
                city.Country = country;
            }

            p.City = city;
            p.CityId = city.Id;
        }

        private async Task GetPlacesAsync(IXLRow row, Excursion e, CancellationToken cancellationToken, string wName)
        {
            int rowNumber = row.RowNumber();
            string valueStr = row.Cell(5).Value.ToString();

            if (string.IsNullOrEmpty(valueStr))
            {
                _isValid = false;
                _validationErrorStr += $"В категорії: {wName}, в рядку: {rowNumber} має бути принаймні одне місце відвідування.\n";
                return;
            }

            var placesArr = valueStr.Split("),");

            for (int i = 0; i < placesArr.Length; i++)
            {
                var plNameWithAdrr = placesArr[i]+")";

                Regex regex = new Regex(@"\S+(?:\s+\S+)*\s*\(\s*\S+(?:\s+\S+)*\s*,\s*\S+(?:\s+\S+)*\s*\)");
                if (!regex.IsMatch(plNameWithAdrr))
                {
                    _isValid = false;
                    _validationErrorStr += $"Помилка в форматі запису місця відвідування екскурсії в категорії: {wName}, в рядку: {rowNumber}.\n";
                    return;
                }

                int startIndex = plNameWithAdrr.IndexOf('(');
                int endIndex = plNameWithAdrr.IndexOf(')');

                string plName= plNameWithAdrr.Substring(0, startIndex).Trim();
                var place = await _context.Places.FirstOrDefaultAsync(pl => pl.Name == plName, cancellationToken);

                if (place is null)
                {
                    place = new Place();
                    place.Name = plName;

                    string cityCountrySubstring = plNameWithAdrr.Substring(startIndex + 1, endIndex - startIndex - 1);

                    await GetCity(place, cityCountrySubstring, cancellationToken);

                    place.Adress = "Deafult adress";

                    _context.Places.Add(place);
                }

                e.Places.Add(place);
            }
        }

    }
}
