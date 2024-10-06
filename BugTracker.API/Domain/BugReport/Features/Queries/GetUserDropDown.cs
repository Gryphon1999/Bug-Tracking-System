using BugTracker.API.Domain.BugReport.Services.Interfaces;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using MediatR;

namespace BugTracker.API.Domain.BugReport.Features.Queries;

public class GetUserDropDown
{
    public class GetUserDropDownQuery : IRequest<ApiResponseHandler<List<AuthUserDto>>>
    {
        public GetUserDropDownQuery()
        {
        }
    }

    public class Handler : IRequestHandler<GetUserDropDownQuery, ApiResponseHandler<List<AuthUserDto>>>
    {
        private readonly IBugReportService _bugReportService;

        public Handler(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        public async Task<ApiResponseHandler<List<AuthUserDto>>> Handle(GetUserDropDownQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _bugReportService.GetUserDropdown();
                return ApiResponseHandler<List<AuthUserDto>>.SuccessResponse(data);
            }
            catch (Exception ex)
            {
                return ApiResponseHandler<List<AuthUserDto>>.ErrorResponse(ex.Message);
            }
        }
    }
}
