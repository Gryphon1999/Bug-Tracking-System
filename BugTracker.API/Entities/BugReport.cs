using BugTracker.Shared.Commons;
using BugTracker.Shared.Enums;
using Sieve.Attributes;

namespace BugTracker.API.Entities;

public class BugReport : BaseEntity
{
    [Sieve(CanFilter = true, CanSort = true)]
    public int BugNo { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public BugSeverity Severity { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public BugStatus Status { get; set; }
    public string ReproductionSteps { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string UserId { get; set; }
    public AuthUser User { get; set; }
    public virtual ICollection<BugAttachment> Attachments { get; set; }
}
