using Microsoft.EntityFrameworkCore;
using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Infrastructure.Persistence;

public class RegistryDbContext : DbContext
{
    public RegistryDbContext(DbContextOptions<RegistryDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<ClinicalVisit> ClinicalVisits => Set<ClinicalVisit>();
    public DbSet<ImagingStudy> ImagingStudies => Set<ImagingStudy>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegistryDbContext).Assembly);
    }
}
