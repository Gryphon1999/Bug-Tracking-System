using BugTracker.API.Domain.BugReport.Services.Interfaces;
using BugTracker.Shared.Commons;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using MediatR;

namespace BugTracker.API.Domain.BugReport.Features.Queries;

public class GetPagedBugReport
{
    public class GetBugReportByIdQuery : IRequest<ApiResponseHandler<PagedListResult<BugReportDto>>>
    {
        public BaseParameterDto parameterDto { get; set; }
        public GetBugReportByIdQuery(BaseParameterDto _parameterDto)
        {
            parameterDto = _parameterDto;
        }
    }

    public class Handler : IRequestHandler<GetBugReportByIdQuery, ApiResponseHandler<PagedListResult<BugReportDto>>>
    {
        private readonly IBugReportService _bugReportService;

        public Handler(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        public async Task<ApiResponseHandler<PagedListResult<BugReportDto>>> Handle(GetBugReportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _bugReportService.GetAllBugReport(request.parameterDto);
                return ApiResponseHandler<PagedListResult<BugReportDto>>.SuccessResponse(data);
            }
            catch (Exception ex)
            {
                return ApiResponseHandler<PagedListResult<BugReportDto>>.ErrorResponse(ex.Message);
            }
        }
    }
}
