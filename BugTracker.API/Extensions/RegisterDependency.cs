using BugTracker.API.Domain.BugReport.Services.Implementations;
using BugTracker.API.Domain.BugReport.Services.Interfaces;
using Sieve.Services;

namespace BugTracker.API.Extensions;

public static class RegisterDependency
{
    public static void AddService(this IServiceCollection services)
    {
        services.AddScoped<SieveProcessor>();
        services.AddScoped<IBugReportService, BugReportService>();
    }
}
