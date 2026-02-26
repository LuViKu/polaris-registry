namespace OphthalmicRegistry.Application.Common;

/// <summary>Holds the authenticated user's identity and claims.</summary>
public class UserContext
{
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public IReadOnlyList<string> Roles { get; init; } = [];
    public Guid? SiteId { get; init; }

    public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

    public bool IsSiteAdmin => IsInRole("SiteAdmin");
    public bool IsClinician => IsInRole("Clinician");
    public bool IsResearcher => IsInRole("Researcher");
    public bool IsSystemAdmin => IsInRole("SystemAdmin");
}
