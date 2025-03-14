namespace Application.Extensions.Filters;

public sealed class RoleFilter : BaseFilter
{
    public string? Name { get; set; }
    public string? Keyword { get; set; }
    public string? Description { get; set; }
}