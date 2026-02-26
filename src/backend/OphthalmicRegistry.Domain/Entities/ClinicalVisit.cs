namespace OphthalmicRegistry.Domain.Entities;

/// <summary>A single longitudinal clinical visit for a patient.</summary>
public class ClinicalVisit
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Patient? Patient { get; private set; }
    public DateOnly VisitDate { get; private set; }
    public string? BestCorrectedVisualAcuity { get; private set; }
    public string? IntraocularPressure { get; private set; }
    public string? ClinicalNotes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<ImagingStudy> ImagingStudies => _imagingStudies.AsReadOnly();
    private readonly List<ImagingStudy> _imagingStudies = new();

    private ClinicalVisit() { }

    public static ClinicalVisit Create(Guid patientId, DateOnly visitDate)
    {
        return new ClinicalVisit
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            VisitDate = visitDate,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void WithClinicalData(string? bestCorrectedVisualAcuity, string? intraocularPressure, string? clinicalNotes)
    {
        BestCorrectedVisualAcuity = bestCorrectedVisualAcuity;
        IntraocularPressure = intraocularPressure;
        ClinicalNotes = clinicalNotes;
    }
}
