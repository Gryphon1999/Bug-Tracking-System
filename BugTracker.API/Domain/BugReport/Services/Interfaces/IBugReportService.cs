using BugTracker.Shared.Commons;
using BugTracker.Shared.Dtos;

namespace BugTracker.API.Domain.BugReport.Services.Interfaces;

public interface IBugReportService
{
    Task AddBugReport(BugReportCreateUpdateDto bugReportCreateUpdateDto, CancellationToken cancellationToken);
    Task UpdateBugReport(string id, BugReportCreateUpdateDto bugReportCreateUpdateDto, CancellationToken cancellationToken);
    Task DeleteBugReport(string id, CancellationToken cancellationToken);
    Task<PagedListResult<BugReportDto>> GetAllBugReport(BaseParameterDto parameters);
    Task<BugReportDto> GetBugReportById(string id, CancellationToken cancellationToken);
    Task<List<AuthUserDto>> GetUserDropdown();
}
