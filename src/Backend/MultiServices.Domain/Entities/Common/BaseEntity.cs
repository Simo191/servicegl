namespace MultiServices.Domain.Entities.Common;

/// <summary>
/// Base entity with audit fields
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Base entity with soft delete
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public int Version { get; set; } = 1;
}
