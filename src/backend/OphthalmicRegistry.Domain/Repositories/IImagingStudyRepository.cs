using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Domain.Repositories;

public interface IImagingStudyRepository
{
    Task<ImagingStudy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ImagingStudy>> GetByVisitAsync(Guid clinicalVisitId, CancellationToken cancellationToken = default);
    Task AddAsync(ImagingStudy study, CancellationToken cancellationToken = default);
}
