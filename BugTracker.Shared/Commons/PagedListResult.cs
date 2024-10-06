namespace BugTracker.Shared.Commons;

public class PagedListResult<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public int NextPageNumber => HasNextPage ? PageNumber + 1 : TotalPages;
    public int PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : 1;
    public IEnumerable<T> Items { get; set; }

    public PagedListResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items;
    }

    public static PagedListResult<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var totalCount = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedListResult<T>(items, totalCount, pageNumber, pageSize);
    }
}

