namespace Application.Extensions.Filters;

public sealed class UserRoleFilter : BaseFilter
{
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
}