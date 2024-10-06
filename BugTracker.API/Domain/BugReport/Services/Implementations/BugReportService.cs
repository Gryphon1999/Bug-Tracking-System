using BugTracker.API.Data;
using BugTracker.API.Domain.BugReport.Services.Interfaces;
using BugTracker.API.Entities;
using BugTracker.Shared.Commons;
using BugTracker.Shared.Constants;
using BugTracker.Shared.Dtos;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace BugTracker.API.Domain.BugReport.Services.Implementations;

public class BugReportService : IBugReportService
{
    private readonly AppDbContext _context;
    private readonly SieveProcessor _sieveProcessor;
    private readonly IWebHostEnvironment _environment;
    public BugReportService(AppDbContext context, IWebHostEnvironment environment, SieveProcessor sieveProcessor)
    {
        _context = context;
        _environment = environment;
        _sieveProcessor = sieveProcessor;
    }

    public async Task AddBugReport(BugReportCreateUpdateDto bugReportCreateUpdateDto, CancellationToken cancellationToken)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var bugReport = bugReportCreateUpdateDto.Adapt<Entities.BugReport>();

                int maxBugNo = _context.BugReports.Any() ? _context.BugReports.Max(x => x.BugNo) : 0;
                bugReport.BugNo = maxBugNo == 0 ? 1 : maxBugNo + 1;

                if (bugReportCreateUpdateDto.BugAttachments != null && bugReportCreateUpdateDto.BugAttachments.Count > 0)
                {
                    var bugAttachments = bugReportCreateUpdateDto.BugAttachments.Adapt<List<BugAttachment>>();
                    bugAttachments.ForEach(x => x.BugReportId = bugReport.Id);
                    await _context.BugAttachments.AddRangeAsync(bugAttachments);
                }

                await _context.BugReports.AddAsync(bugReport);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception(ex.Message);
            }
        }
    }

    public async Task DeleteBugReport(string id, CancellationToken cancellationToken)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var bugReport = await _context.BugReports.FindAsync(id, cancellationToken);
                if (bugReport == null)
                {
                    throw new KeyNotFoundException($"Bug No: {bugReport.BugNo} not found.");
                }

                var existingAttachments = await _context.BugAttachments
                            .Where(ba => ba.BugReportId == bugReport.Id)
                            .ToListAsync(cancellationToken);

                if (existingAttachments.Count > 0)
                {
                    _context.BugAttachments.RemoveRange(existingAttachments);
                }

                _context.BugReports.Remove(bugReport);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception(ex.Message);
            }
        }
    }

    public async Task<PagedListResult<BugReportDto>> GetAllBugReport(BaseParameterDto parameters)
    {
        try
        {
            var query = _context.BugReports.AsQueryable();

            var sieveModel = new SieveModel
            {
                Filters = parameters.Filters,
            };

            var filteredList = _sieveProcessor.Apply(sieveModel, query);

            var dtoCollection = filteredList.Select(x => new BugReportDto
            {
                Id = x.Id,
                BugNo = x.BugNo,
                Title = x.Title,
                Description = x.Description,
                Severity = x.Severity,
                Status = x.Status,
                ReproductionSteps = x.ReproductionSteps,
                UserId = x.UserId,
                UserName = x.User.Name,
            }).OrderBy(x=>x.BugNo);
            return PagedListResult<BugReportDto>.Create(dtoCollection, parameters.PageNumber, parameters.PageSize);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<BugReportDto> GetBugReportById(string id, CancellationToken cancellationToken)
    {
        try
        {
            var bugReport = await _context.BugReports
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (bugReport == null)
            {
                throw new KeyNotFoundException($"Bug No: {bugReport.BugNo} not found.");
            }
            var result = bugReport.Adapt<BugReportDto>();
            result.UserName = bugReport.User.Name;

            var filePaths = await _context.BugAttachments
                .Where(x => x.BugReportId == bugReport.Id)
                .Select(x => x.FileName)
                .ToListAsync();

            if (filePaths.Count > 0)
            {
                result.FileName.AddRange(filePaths);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<AuthUserDto>> GetUserDropdown()
    {
        var developerRoleId = await _context.Roles
        .Where(r => r.Name == DefaultRoleConstant.Developer)
        .Select(r => r.Id)
        .FirstOrDefaultAsync();

        var usersInDeveloperRole = await _context.UserRoles
            .Where(ur => ur.RoleId == developerRoleId)
            .Select(ur => ur.UserId)
            .ToListAsync();

        return await _context.Users
            .Where(u => usersInDeveloperRole.Contains(u.Id))
            .Select(u => new AuthUserDto
            {
                Id = u.Id,
                UserName = u.Name
            }).ToListAsync();
    }

    public async Task UpdateBugReport(string id, BugReportCreateUpdateDto bugReportCreateUpdateDto, CancellationToken cancellationToken)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var bugReport = await _context.BugReports.FindAsync(id, cancellationToken);
                if (bugReport == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new KeyNotFoundException($"Bug No: {bugReport.BugNo} not found.");
                }
                bugReportCreateUpdateDto.Id = bugReport.Id;
                bugReportCreateUpdateDto.Adapt(bugReport);

                if (bugReportCreateUpdateDto.BugAttachments != null && bugReportCreateUpdateDto.BugAttachments.Count > 0)
                {
                    var existingAttachments = await _context.BugAttachments
                        .Where(ba => ba.BugReportId == bugReport.Id)
                        .ToListAsync(cancellationToken);

                    if (existingAttachments.Count > 0)
                    {
                        _context.BugAttachments.RemoveRange(existingAttachments);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    var bugAttachments = bugReportCreateUpdateDto.BugAttachments.Adapt<List<BugAttachment>>();
                    bugAttachments.ForEach(x => x.BugReportId = bugReport.Id);
                    await _context.BugAttachments.AddRangeAsync(bugAttachments);
                }
                _context.BugReports.Update(bugReport);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception(ex.Message);
            }
        }
    }
}
