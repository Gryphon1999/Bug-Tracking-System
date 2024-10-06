using BugTracker.API.Domain.BugReport.Services.Interfaces;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using MediatR;

namespace BugTracker.API.Domain.BugReport.Features.Queries;

public class GetBugReportById
{
    public class GetBugReportByIdQuery : IRequest<ApiResponseHandler<BugReportDto>>
    {
        public string Id { get; set; }
        public GetBugReportByIdQuery(string id)
        {
            Id = id;
        }
    }

    public class Handler : IRequestHandler<GetBugReportByIdQuery, ApiResponseHandler<BugReportDto>>
    {
        private readonly IBugReportService _bugReportService;

        public Handler(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        public async Task<ApiResponseHandler<BugReportDto>> Handle(GetBugReportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _bugReportService.GetBugReportById(request.Id, cancellationToken);
                return ApiResponseHandler<BugReportDto>.SuccessResponse(data);
            }
            catch (Exception ex)
            {
                return ApiResponseHandler<BugReportDto>.ErrorResponse(ex.Message);
            }
        }
    }
}
