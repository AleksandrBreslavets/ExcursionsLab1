using ExcursionsDomain.Model;

namespace ExcursionsInfrastructure.Services
{
    public class ExcursionDataPortServiceFactory
       : IDataPortServiceFactory<Excursion>

    {
        private readonly ExcursionsDbContext _context;
        public ExcursionDataPortServiceFactory(ExcursionsDbContext context)
        {
            _context = context;
        }
        public IImportService<Excursion> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new ExcursionImportService(_context);
            }
            throw new NotImplementedException($"Не розроблений сервіс імпорту екскурсій з типом контнету {contentType}");
        }
        public IExportService<Excursion> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new ExcursionExportService(_context);
            }
            throw new NotImplementedException($"Не розроблений сервіс імпорту екскурсій з типом контнету {contentType}");
        }

    }
}
