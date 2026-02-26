namespace OphthalmicRegistry.Domain.Entities;

/// <summary>Represents a multimodal imaging study (OCT, FAF, fundus, etc.).</summary>
public class ImagingStudy
{
    public Guid Id { get; private set; }
    public Guid ClinicalVisitId { get; private set; }
    public ClinicalVisit? ClinicalVisit { get; private set; }
    public string Modality { get; private set; } = string.Empty;
    public string? OrthancStudyId { get; private set; }
    public string? MinioObjectKey { get; private set; }
    public DateTimeOffset AcquiredAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ImagingStudy() { }

    public static ImagingStudy Create(Guid clinicalVisitId, string modality, DateTimeOffset acquiredAt)
    {
        return new ImagingStudy
        {
            Id = Guid.NewGuid(),
            ClinicalVisitId = clinicalVisitId,
            Modality = modality,
            AcquiredAt = acquiredAt,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}
