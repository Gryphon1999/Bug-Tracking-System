using BugTracker.Shared.Commons;

namespace BugTracker.API.Entities;

public class BugAttachment : BaseEntity
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string BugReportId { get; set; } 
    public BugReport BugReport { get; set; }
}
