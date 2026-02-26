using Microsoft.EntityFrameworkCore;
using OphthalmicRegistry.Domain.Entities;
using OphthalmicRegistry.Domain.Repositories;
using OphthalmicRegistry.Infrastructure.Persistence;

namespace OphthalmicRegistry.Infrastructure.Persistence.Repositories;

public class ImagingStudyRepository : IImagingStudyRepository
{
    private readonly RegistryDbContext _context;

    public ImagingStudyRepository(RegistryDbContext context) => _context = context;

    public async Task<ImagingStudy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ImagingStudies.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<ImagingStudy>> GetByVisitAsync(Guid clinicalVisitId, CancellationToken cancellationToken = default)
        => await _context.ImagingStudies
            .Where(s => s.ClinicalVisitId == clinicalVisitId)
            .OrderByDescending(s => s.AcquiredAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ImagingStudy study, CancellationToken cancellationToken = default)
    {
        await _context.ImagingStudies.AddAsync(study, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
