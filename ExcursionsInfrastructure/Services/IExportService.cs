using ExcursionsDomain.Model;

namespace ExcursionsInfrastructure.Services
{
    public interface IExportService<TEntity>
    where TEntity : Entity
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken, List<int> collection, List<int> selectedCategories);
    }
}
