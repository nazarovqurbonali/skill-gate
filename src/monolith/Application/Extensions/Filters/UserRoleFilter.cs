namespace Application.Extensions.Filters;

public sealed record UserRoleFilter(
    Guid? UserId,
    Guid? RoleId):BaseFilter;