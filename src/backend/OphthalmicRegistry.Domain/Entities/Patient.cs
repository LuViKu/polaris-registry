namespace OphthalmicRegistry.Domain.Entities;

/// <summary>Represents a pseudonymized patient record.</summary>
public class Patient
{
    public Guid Id { get; private set; }

    /// <summary>HMAC-pseudonymized identifier derived from hospital MRN.</summary>
    public string PseudonymizedId { get; private set; } = string.Empty;

    public DateOnly DateOfBirth { get; private set; }
    public string Sex { get; private set; } = string.Empty;
    public Guid SiteId { get; private set; }
    public Site? Site { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<ClinicalVisit> ClinicalVisits => _clinicalVisits.AsReadOnly();
    private readonly List<ClinicalVisit> _clinicalVisits = new();

    private Patient() { }

    public static Patient Create(string pseudonymizedId, DateOnly dateOfBirth, string sex, Guid siteId)
    {
        return new Patient
        {
            Id = Guid.NewGuid(),
            PseudonymizedId = pseudonymizedId,
            DateOfBirth = dateOfBirth,
            Sex = sex,
            SiteId = siteId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
    }
}
