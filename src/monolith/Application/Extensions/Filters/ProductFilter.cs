namespace Application.Extensions.Filters;

public sealed record ProductFilter(
    string? Name,
    decimal? Price,
    string? Description,
    int? StockQuantity) : BaseFilter;