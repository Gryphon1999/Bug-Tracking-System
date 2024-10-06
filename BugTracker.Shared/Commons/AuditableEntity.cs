namespace BugTracker.Shared.Commons;

public abstract class AuditableEntity
{
    public string CreatedBy { get; private set; }
    public DateTime? CreatedOn { get; private set; }
    public string ModifiedBy { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public void SetCreatedInfo(string createdBy)
    {
        CreatedBy = createdBy;
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = null;
    }

    public void SetModifiedInfo(string modifiedBy)
    {
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }
}
