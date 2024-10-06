using System.ComponentModel.DataAnnotations;

namespace BugTracker.Shared.Commons;

public abstract class BaseEntity : AuditableEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
