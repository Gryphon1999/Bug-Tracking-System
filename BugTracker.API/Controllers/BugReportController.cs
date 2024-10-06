using BugTracker.API.Domain.BugReport.Features.Commands;
using BugTracker.API.Domain.BugReport.Features.Queries;
using BugTracker.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.API.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class BugReportController : ControllerBase
{
    private readonly IMediator _mediator;
    public BugReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("GetPagedBugReport")]
    public async Task<IActionResult> GetPagedBugReport([FromQuery] BaseParameterDto parameterDto)
    {
        var query = new GetPagedBugReport.GetBugReportByIdQuery(parameterDto);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetBugReportById/{id}")]
    public async Task<IActionResult> GetBugReportById(string id)
    {
        var query = new GetBugReportById.GetBugReportByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddBugReport")]
    public async Task<IActionResult> AddBugReport([FromBody] BugReportCreateUpdateDto bugReportDto)
    {
        var query = new AddBugReport.AddBugReportCommand(bugReportDto);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("UpdateBugReport/{id}")]
    public async Task<IActionResult> UpdateBugReport(string id, [FromBody] BugReportCreateUpdateDto bugReportDto)
    {
        var query = new UpdateBugReport.UpdateBugReportCommand(id, bugReportDto);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete("DeleteBugReport/{id}")]
    public async Task<IActionResult> DeleteBugReport(string id)
    {
        var query = new DeleteBugReport.DeleteBugReportCommand(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetUserDropdown")]
    public async Task<IActionResult> GetUserDropdown()
    {
        var query = new GetUserDropDown.GetUserDropDownQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
