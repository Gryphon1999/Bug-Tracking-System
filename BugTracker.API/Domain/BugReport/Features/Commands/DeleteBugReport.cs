using BugTracker.API.Domain.BugReport.Services.Interfaces;
using BugTracker.Shared.Helper;
using MediatR;

namespace BugTracker.API.Domain.BugReport.Features.Commands;

public class DeleteBugReport
{
    public class DeleteBugReportCommand : IRequest<ApiResponseHandler<string>>
    {
        public string Id { get; set; }
        public DeleteBugReportCommand(string id)
        {
            Id = id;
        }
    }

    public class Handler : IRequestHandler<DeleteBugReportCommand, ApiResponseHandler<string>>
    {
        private readonly IBugReportService _bugReportService;

        public Handler(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        public async Task<ApiResponseHandler<string>> Handle(DeleteBugReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _bugReportService.DeleteBugReport(request.Id, cancellationToken);
                return ApiResponseHandler<string>.SuccessResponse("Bug has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHandler<string>.ErrorResponse(ex.Message);
            }
        }
    }
}
