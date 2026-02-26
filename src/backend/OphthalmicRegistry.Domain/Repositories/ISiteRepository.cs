using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Domain.Repositories;

public interface ISiteRepository
{
    Task<Site?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Site site, CancellationToken cancellationToken = default);
}
