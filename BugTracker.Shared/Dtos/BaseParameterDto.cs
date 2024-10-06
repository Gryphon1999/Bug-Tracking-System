namespace BugTracker.Shared.Dtos;

public class BaseParameterDto
{
    public string Filters { get; set; }
    public int PageNumber { get; set; } = 1; 
    public int PageSize { get; set; } = 10; 
}
