using ExcursionsDomain.Model;

namespace ExcursionsInfrastructure.Services
{
    public interface IImportService<TEntity>
    where TEntity : Entity
    {
        Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }
}
