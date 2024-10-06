using BugTracker.API.Domain.BugReport.Services.Interfaces;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using MediatR;

namespace BugTracker.API.Domain.BugReport.Features.Commands;

public class UpdateBugReport
{
    public class UpdateBugReportCommand : IRequest<ApiResponseHandler<string>>
    {
        public string Id { get; set; }
        public BugReportCreateUpdateDto BugReportDto { get; set; }
        public UpdateBugReportCommand(string id, BugReportCreateUpdateDto bugReportDto)
        {
            Id = id;
            BugReportDto = bugReportDto;
        }
    }

    public class Handler : IRequestHandler<UpdateBugReportCommand, ApiResponseHandler<string>>
    {
        private readonly IBugReportService _bugReportService;

        public Handler(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        public async Task<ApiResponseHandler<string>> Handle(UpdateBugReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _bugReportService.UpdateBugReport(request.Id, request.BugReportDto, cancellationToken);
                return ApiResponseHandler<string>.SuccessResponse("Bug has been updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHandler<string>.ErrorResponse(ex.Message);
            }
        }
    }
}
