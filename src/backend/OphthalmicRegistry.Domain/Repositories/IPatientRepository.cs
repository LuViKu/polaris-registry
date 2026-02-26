using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Domain.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Patient?> GetByPseudonymizedIdAsync(string pseudonymizedId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> GetBySiteAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
}
