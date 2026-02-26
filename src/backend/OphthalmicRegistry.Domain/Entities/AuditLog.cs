namespace OphthalmicRegistry.Domain.Entities;

/// <summary>Immutable audit log entry for GDPR compliance.</summary>
public class AuditLog
{
    public long Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public string? Details { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(string userId, string action, string entityType, Guid? entityId, string ipAddress, string? details = null)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            IpAddress = ipAddress,
            Details = details,
            OccurredAt = DateTimeOffset.UtcNow,
        };
    }
}
