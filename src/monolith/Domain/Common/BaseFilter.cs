namespace Domain.Common;

public abstract class BaseFilter
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }

    protected BaseFilter()
    {
        PageNumber = 1;
        PageSize = 10;
    }

    protected BaseFilter(int pageSize, int pageNumber)
    {
        PageSize = pageSize <= 0 ? 10 : pageSize;
        PageNumber = pageNumber <= 0 ? 1 : pageNumber;
    }
}