using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OphthalmicRegistry.Domain.Entities;

namespace OphthalmicRegistry.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PseudonymizedId).IsRequired().HasMaxLength(128);
        builder.HasIndex(p => p.PseudonymizedId).IsUnique();
        builder.Property(p => p.Sex).IsRequired().HasMaxLength(20);
        builder.HasOne(p => p.Site).WithMany(s => s.Patients).HasForeignKey(p => p.SiteId);
    }
}
