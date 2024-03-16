using ClosedXML.Excel;
using ExcursionsDomain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace ExcursionsInfrastructure.Services
{
    public class ExcursionExportService : IExportService<ExcursionsDomain.Model.Excursion>
    {
        private readonly ExcursionsDbContext _context;

        public ExcursionExportService(ExcursionsDbContext context)
        {
            _context = context;
        }

        private static readonly IReadOnlyList<string> HeaderNames =
           new string[]
           {
                "Назва",
                "Дата проведення",
                "Макс. кількість відвідувачів",
                "Всі категорії",
                "Місця проведення",
                "Опис",
                "Ціна",
                "Тривалість"
           };


        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
                worksheet.Column(columnIndex + 1).Width = 25;
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private void WriteExcursion(IXLWorksheet worksheet, Excursion e, int rowIndex)
        {
            var columnIndex = 1;
            List<string> places= new List<string>();

            foreach(var p in e.Places)
            {
                string place = $"{p.Name}({p.City.Name}, {p.City.Country.Name})";
                places.Add(place);
            }

            worksheet.Cell(rowIndex, columnIndex++).Value = e.Name;
            worksheet.Cell(rowIndex, columnIndex++).Value = e.Date.ToString("MM/dd/yyyy HH:mm:ss");
            worksheet.Cell(rowIndex, columnIndex++).Value = e.MaxPeopleAmount;
            worksheet.Cell(rowIndex, columnIndex++).Value = string.Join(",", e.Categories.Select(c=>c.Name));
            worksheet.Cell(rowIndex, columnIndex++).Value = string.Join(",", places);
            worksheet.Cell(rowIndex, columnIndex++).Value = e.Description;
            worksheet.Cell(rowIndex, columnIndex++).Value = e.Price;
            worksheet.Cell(rowIndex, columnIndex++).Value = e.Duration;
        }

        private void WriteExcursions(IXLWorksheet worksheet, ICollection<Excursion> excursions)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var e in excursions)
            {
                WriteExcursion(worksheet, e, rowIndex);
                worksheet.Row(rowIndex).Height = 40;
                rowIndex++;
            }
        }


        private void WriteCategories(XLWorkbook workbook, ICollection<Category> categories)
        {
            foreach (var cat in categories)
            {
                if (cat is not null)
                {
                    var worksheet = workbook.Worksheets.Add(cat.Name);
                    WriteExcursions(worksheet, cat.Excursions.ToList());
                }
            }
        }


        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken, List<int> collection, List<int> selectedCategories)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Проблеми з записом до файлу.");
            }

            var categories = await _context.Categories
            .Include(category => category.Excursions)
                .ThenInclude(e => e.Places)
                .ThenInclude(p=>p.City)
                .ThenInclude(c=>c.Country)
            .Include(category => category.Excursions)
                .ThenInclude(e => e.Categories)
             .ToListAsync(cancellationToken);

            if (selectedCategories.Count>0)
            {
                categories = categories.Where(category => selectedCategories.Contains(category.Id)).ToList();
            }
            else
            {
                categories = categories
                    .Where(category => category.Excursions.Any(excursion => collection.Contains(excursion.Id)))
                    .ToList();
            } 

            foreach (var category in categories)
            {
                category.Excursions = category.Excursions.Where(excursion => collection.Contains(excursion.Id)).ToList();
            }

            var workbook = new XLWorkbook();

           WriteCategories(workbook, categories);
           workbook.SaveAs(stream);
        }


    }
}