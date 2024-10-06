using BugTracker.Shared.Enums;

namespace BugTracker.Shared.Dtos;

public class BugReportDto
{
    public string Id { get; set; }
    public int BugNo { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public BugSeverity Severity { get; set; }
    public BugStatus Status { get; set; }
    public string ReproductionSteps { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<string> FileName { get; set; } = new();
}
