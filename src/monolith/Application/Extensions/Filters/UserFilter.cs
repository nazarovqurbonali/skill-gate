namespace Application.Extensions.Filters;

public sealed record UserFilter(
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    string? UserName):BaseFilter;