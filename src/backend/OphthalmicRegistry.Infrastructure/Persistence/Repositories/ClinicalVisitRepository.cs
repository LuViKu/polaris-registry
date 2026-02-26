using Microsoft.EntityFrameworkCore;
using OphthalmicRegistry.Domain.Entities;
using OphthalmicRegistry.Domain.Repositories;
using OphthalmicRegistry.Infrastructure.Persistence;

namespace OphthalmicRegistry.Infrastructure.Persistence.Repositories;

public class ClinicalVisitRepository : IClinicalVisitRepository
{
    private readonly RegistryDbContext _context;

    public ClinicalVisitRepository(RegistryDbContext context) => _context = context;

    public async Task<ClinicalVisit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ClinicalVisits.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<ClinicalVisit>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
        => await _context.ClinicalVisits
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.VisitDate)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ClinicalVisit visit, CancellationToken cancellationToken = default)
    {
        await _context.ClinicalVisits.AddAsync(visit, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
