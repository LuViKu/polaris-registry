namespace OphthalmicRegistry.Domain.Entities;

/// <summary>Represents a participating hospital / clinical site.</summary>
public class Site
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<Patient> Patients => _patients.AsReadOnly();
    private readonly List<Patient> _patients = new();

    private Site() { }

    public static Site Create(string name, string country)
    {
        return new Site
        {
            Id = Guid.NewGuid(),
            Name = name,
            Country = country,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}
