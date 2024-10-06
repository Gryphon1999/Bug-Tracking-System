using BugTracker.API.Domain.BugReport.Services.Interfaces;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using MediatR;

namespace BugTracker.API.Domain.BugReport.Features.Commands;

public class AddBugReport
{
    public class AddBugReportCommand : IRequest<ApiResponseHandler<string>>
    {
        public BugReportCreateUpdateDto BugReportDto { get; set; }
        public AddBugReportCommand(BugReportCreateUpdateDto bugReportDto)
        {
            BugReportDto = bugReportDto;
        }
    }

    public class Handler : IRequestHandler<AddBugReportCommand, ApiResponseHandler<string>>
    {
        private readonly IBugReportService _bugReportService;

        public Handler(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        public async Task<ApiResponseHandler<string>> Handle(AddBugReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _bugReportService.AddBugReport(request.BugReportDto, cancellationToken);
                return ApiResponseHandler<string>.SuccessResponse("Bug has been added successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHandler<string>.ErrorResponse(ex.Message);
            }
        }
    }
}
