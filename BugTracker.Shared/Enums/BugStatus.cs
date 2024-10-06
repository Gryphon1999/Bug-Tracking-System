using System.ComponentModel.DataAnnotations;

namespace BugTracker.Shared.Enums;

public enum BugStatus
{
    [Display(Name = "Open")]
    Open,
    [Display(Name = "In Process")]
    InProgress,
    [Display(Name = "Resolve")]
    Resolved,
    [Display(Name = "Closed")]
    Closed
}
