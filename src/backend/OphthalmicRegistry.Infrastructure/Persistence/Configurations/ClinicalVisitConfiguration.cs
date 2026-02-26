using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Infrastructure.Persistence.Configurations;

public class ClinicalVisitConfiguration : IEntityTypeConfiguration<ClinicalVisit>
{
    public void Configure(EntityTypeBuilder<ClinicalVisit> builder)
    {
        builder.HasKey(v => v.Id);
        builder.HasOne(v => v.Patient).WithMany(p => p.ClinicalVisits).HasForeignKey(v => v.PatientId);
        builder.Property(v => v.BestCorrectedVisualAcuity).HasMaxLength(50);
        builder.Property(v => v.IntraocularPressure).HasMaxLength(50);
    }
}
