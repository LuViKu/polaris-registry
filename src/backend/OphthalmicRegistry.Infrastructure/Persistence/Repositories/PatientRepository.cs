using Microsoft.EntityFrameworkCore;
using OphthalmicRegistry.Domain.Entities;
using OphthalmicRegistry.Domain.Repositories;
using OphthalmicRegistry.Infrastructure.Persistence;

namespace OphthalmicRegistry.Infrastructure.Persistence.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly RegistryDbContext _context;

    public PatientRepository(RegistryDbContext context) => _context = context;

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Patients.FindAsync(new object[] { id }, cancellationToken);

    public async Task<Patient?> GetByPseudonymizedIdAsync(string pseudonymizedId, CancellationToken cancellationToken = default)
        => await _context.Patients.FirstOrDefaultAsync(p => p.PseudonymizedId == pseudonymizedId, cancellationToken);

    public async Task<IReadOnlyList<Patient>> GetBySiteAsync(Guid siteId, CancellationToken cancellationToken = default)
        => await _context.Patients.Where(p => p.SiteId == siteId).ToListAsync(cancellationToken);

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
