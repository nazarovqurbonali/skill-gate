namespace Application.Extensions.Filters;

public sealed record RoleFilter(
    string? Name,
    string? Keyword,
    string? Description):BaseFilter;