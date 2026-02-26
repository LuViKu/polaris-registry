using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Infrastructure.Persistence.Configurations;

public class ImagingStudyConfiguration : IEntityTypeConfiguration<ImagingStudy>
{
    public void Configure(EntityTypeBuilder<ImagingStudy> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Modality).IsRequired().HasMaxLength(50);
        builder.Property(i => i.OrthancStudyId).HasMaxLength(64);
        builder.Property(i => i.MinioObjectKey).HasMaxLength(500);
        builder.HasOne(i => i.ClinicalVisit).WithMany(v => v.ImagingStudies).HasForeignKey(i => i.ClinicalVisitId);
    }
}
