namespace Application.Extensions.Filters;

public sealed class UserFilter : BaseFilter
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserName { get; set; }
}
