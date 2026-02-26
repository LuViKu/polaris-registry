using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Domain.Repositories;

public interface IClinicalVisitRepository
{
    Task<ClinicalVisit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClinicalVisit>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task AddAsync(ClinicalVisit visit, CancellationToken cancellationToken = default);
}
