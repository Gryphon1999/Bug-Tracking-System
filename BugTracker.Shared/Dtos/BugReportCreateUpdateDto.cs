using BugTracker.Shared.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BugTracker.Shared.Dtos;

public class BugReportCreateUpdateDto
{
    [JsonIgnore]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    [Required]
    public BugSeverity Severity { get; set; }
    [Required]
    public BugStatus Status { get; set; }
    public string ReproductionSteps { get; set; }
    public string UserId { get; set; }
    public IFormFileCollection Attachments { get; set; }
    public List<BugAttachmentCreateUpdateDto> BugAttachments { get; set; } = new();
}
