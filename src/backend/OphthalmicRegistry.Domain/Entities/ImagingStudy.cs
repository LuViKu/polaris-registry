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
    /// <summary>Original file name as uploaded by the user.</summary>
    public string? OriginalFileName { get; private set; }
    /// <summary>File format: "dicom" for DICOM files, or the native extension (e.g. "e2e", "fds", "img").</summary>
    public string FileFormat { get; private set; } = string.Empty;
    public DateTimeOffset AcquiredAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ImagingStudy() { }

    public static ImagingStudy Create(Guid clinicalVisitId, string modality, string fileFormat, DateTimeOffset acquiredAt, string? originalFileName = null)
    {
        return new ImagingStudy
        {
            Id = Guid.NewGuid(),
            ClinicalVisitId = clinicalVisitId,
            Modality = modality,
            FileFormat = fileFormat,
            OriginalFileName = originalFileName,
            AcquiredAt = acquiredAt,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void SetOrthancStudyId(string orthancStudyId)
    {
        OrthancStudyId = orthancStudyId;
    }

    public void SetMinioObjectKey(string minioObjectKey)
    {
        MinioObjectKey = minioObjectKey;
    }
}
