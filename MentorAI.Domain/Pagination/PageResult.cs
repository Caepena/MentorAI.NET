namespace MonitoringMottu.Domain.Pagination;

public class PageResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public long Total { get; set; }
}